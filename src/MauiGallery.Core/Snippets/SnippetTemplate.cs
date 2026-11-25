using System.Text.RegularExpressions;

namespace MauiGallery.Core.Snippets;

/// <summary>
/// A code sample with <c>$(Name)</c> placeholders that are replaced by the live values of a sample's options.
/// </summary>
/// <remarks>
/// A placeholder whose value is <see langword="null"/> removes the whole line it is on. Use this to omit
/// properties that are at their default value, so the rendered code only shows what the reader changed.
/// When a removed line closes a XAML element (ends in <c>/&gt;</c> or <c>&gt;</c>), the terminator moves to the
/// previous line so the markup stays well-formed.
/// </remarks>
public sealed partial class SnippetTemplate
{
    private readonly string[] _lines;

    public SnippetTemplate(string source)
    {
        ArgumentNullException.ThrowIfNull(source);

        _lines = source.ReplaceLineEndings("\n").TrimEnd('\n').Split('\n');
        Placeholders = [.. PlaceholderPattern().Matches(source).Select(m => m.Groups["name"].Value).Distinct(StringComparer.Ordinal)];
    }

    /// <summary>The distinct placeholder names used in the template, in order of first appearance.</summary>
    public IReadOnlyList<string> Placeholders { get; }

    /// <summary>Renders the template using the supplied placeholder values.</summary>
    /// <exception cref="KeyNotFoundException">A placeholder in the template has no entry in <paramref name="values"/>.</exception>
    public string Render(IReadOnlyDictionary<string, string?> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var output = new List<string>(_lines.Length);
        foreach (var line in _lines)
        {
            var omitLine = false;
            var rendered = PlaceholderPattern().Replace(line, match =>
            {
                var name = match.Groups["name"].Value;
                if (!values.TryGetValue(name, out var value))
                {
                    throw new KeyNotFoundException($"No value was supplied for placeholder '$({name})'.");
                }

                omitLine |= value is null;
                return value ?? string.Empty;
            });

            if (!omitLine)
            {
                output.Add(rendered);
            }
            else if (output.Count > 0 && ElementTerminator(line) is { } terminator)
            {
                output[^1] = output[^1].TrimEnd() + terminator;
            }
        }

        return string.Join('\n', output);
    }

    private static string? ElementTerminator(string line)
    {
        var trimmed = line.TrimEnd();
        if (trimmed.TrimStart().StartsWith('<'))
        {
            return null;
        }

        return trimmed.EndsWith("/>", StringComparison.Ordinal) ? " />"
            : trimmed.EndsWith('>') ? ">"
            : null;
    }

    [GeneratedRegex(@"\$\((?<name>[A-Za-z][A-Za-z0-9]*)\)")]
    private static partial Regex PlaceholderPattern();
}
