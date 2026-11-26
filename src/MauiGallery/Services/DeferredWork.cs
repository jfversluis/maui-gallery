namespace MauiGallery.Services;

/// <summary>
/// Runs UI work that can wait, one item at a time and a frame apart, in the order it was queued. What is already on
/// screen gets drawn first and input is handled in between, so the app stays responsive while, for example, a page's
/// examples fill in.
/// </summary>
/// <remarks>Everything runs on the UI thread, which all windows share.</remarks>
public static class DeferredWork
{
    // Long enough for the platform to draw a frame between two items.
    private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(16);

    private static readonly Queue<(IDispatcher Dispatcher, Func<bool> Work)> Pending = new();
    private static bool _isRunning;

    /// <summary>
    /// Queues <paramref name="work"/>. It returns <see langword="false"/> when it turned out to have nothing to do,
    /// so the next item runs without waiting for another frame.
    /// </summary>
    public static void Enqueue(IDispatcher dispatcher, Func<bool> work)
    {
        ArgumentNullException.ThrowIfNull(dispatcher);
        ArgumentNullException.ThrowIfNull(work);

        Pending.Enqueue((dispatcher, work));
        if (!_isRunning)
        {
            _isRunning = true;
            dispatcher.DispatchDelayed(Interval, RunNext);
        }
    }

    private static void RunNext()
    {
        while (Pending.TryDequeue(out var item))
        {
            if (item.Work())
            {
                break;
            }
        }

        if (Pending.TryPeek(out var next))
        {
            next.Dispatcher.DispatchDelayed(Interval, RunNext);
        }
        else
        {
            _isRunning = false;
            PageLoadTrace.EndDeferred("settled");
        }
    }
}
