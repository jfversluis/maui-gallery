using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace MauiGallery.Services;

/// <summary>
/// Measures how long a page takes from the navigation request until it has been laid out, and writes one line per
/// navigation, for example <c>[PageLoad] button 412 ms: resolve 3, construct 260, handlers 90, layout 55 | code 31 (8x)</c>.
/// Work that continues after that, such as examples filling in, is reported on a second line.
/// </summary>
/// <remarks>
/// Milestones are time since the previous milestone; the part after the bar adds up work that was timed with
/// <see cref="Measure"/> (it overlaps the milestones). The line goes to the console, which reaches DevFlow logs, the
/// macOS/iOS unified log and Android's logcat in Debug and Release builds alike. Everything runs on the UI thread.
/// </remarks>
public static class PageLoadTrace
{
    private static readonly List<(string Name, long Timestamp)> Milestones = [];
    private static readonly Dictionary<string, (TimeSpan Time, int Count)> Work = new(StringComparer.Ordinal);
    private static string? _route;
    private static long _start;
    private static bool _ended;

    /// <summary>Whether a navigation is being measured.</summary>
    public static bool IsActive => _route is not null;

    /// <summary>
    /// Identifies the navigation being measured, so a page can ignore its own events (such as being removed) once a
    /// later navigation has started.
    /// </summary>
    public static int Navigation { get; private set; }

    /// <summary>Starts measuring a navigation. Any measurement still in progress is discarded.</summary>
    public static void Begin(string route)
    {
        Milestones.Clear();
        Work.Clear();
        _route = route;
        _ended = false;
        Navigation++;
        _start = Stopwatch.GetTimestamp();
    }

    /// <summary>Records a milestone, such as "construct", for the navigation being measured.</summary>
    public static void Mark(string milestone)
    {
        if (IsActive && !_ended)
        {
            Milestones.Add((milestone, Stopwatch.GetTimestamp()));
        }
    }

    /// <summary>Adds the time until the returned scope is disposed to the named work, such as "code".</summary>
    public static WorkScope Measure(string work) => new(IsActive ? work : null, Stopwatch.GetTimestamp());

    /// <summary>Records the final milestone, once the page is on screen, and writes the measurement.</summary>
    public static void End(string milestone)
    {
        if (!IsActive || _ended)
        {
            return;
        }

        Mark(milestone);
        _ended = true;
        var line = new StringBuilder("[PageLoad] ")
            .Append(_route).Append(' ')
            .Append(Milliseconds(_start, Milestones[^1].Timestamp)).Append(" ms:");
        var previous = _start;
        foreach (var (name, timestamp) in Milestones)
        {
            line.Append(' ').Append(name).Append(' ').Append(Milliseconds(previous, timestamp)).Append(',');
            previous = timestamp;
        }

        line.Length--;
        Write(line);
    }

    /// <summary>Reports work that finished after the page appeared, such as its examples filling in.</summary>
    public static void EndDeferred(string milestone)
    {
        if (!_ended || !IsActive)
        {
            Mark(milestone);
            return;
        }

        var line = new StringBuilder("[PageLoad] ")
            .Append(_route).Append(' ').Append(milestone).Append(" after ")
            .Append(Milliseconds(_start, Stopwatch.GetTimestamp())).Append(" ms");
        Write(line);
        _route = null;
    }

    private static void Write(StringBuilder line)
    {
        if (Work.Count > 0)
        {
            line.Append(" |");
            foreach (var (name, (time, count)) in Work)
            {
                line.Append(CultureInfo.InvariantCulture, $" {name} {time.TotalMilliseconds:0} ({count}x)");
            }

            Work.Clear();
        }

        Console.WriteLine(line.ToString());
    }

    private static string Milliseconds(long from, long to) =>
        Stopwatch.GetElapsedTime(from, to).TotalMilliseconds.ToString("0", CultureInfo.InvariantCulture);

    /// <summary>Times one piece of work; see <see cref="Measure"/>.</summary>
    public readonly struct WorkScope(string? work, long start) : IDisposable
    {
        public void Dispose()
        {
            if (work is null || !IsActive)
            {
                return;
            }

            var elapsed = Stopwatch.GetElapsedTime(start);
            Work[work] = Work.TryGetValue(work, out var total) ? (total.Time + elapsed, total.Count + 1) : (elapsed, 1);
        }
    }
}
