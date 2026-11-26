namespace MauiGallery.Core.Syntax;

/// <summary>Splits source code into classified tokens for display. Concatenating the tokens reproduces the input exactly.</summary>
public static class SyntaxHighlighter
{
    public static IReadOnlyList<SyntaxToken> Tokenize(string source, CodeLanguage language)
    {
        ArgumentNullException.ThrowIfNull(source);

        var tokens = language switch
        {
            CodeLanguage.Xaml => XamlTokenizer.Tokenize(source),
            CodeLanguage.CSharp => CSharpTokenizer.Tokenize(source),
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null),
        };

        return Merge(tokens);
    }

    /// <summary>
    /// The tokens that make up the first <paramref name="maxLines"/> lines, for a collapsed preview of long code.
    /// The line break after the last line is left out.
    /// </summary>
    public static IReadOnlyList<SyntaxToken> TakeLines(IReadOnlyList<SyntaxToken> tokens, int maxLines)
    {
        ArgumentNullException.ThrowIfNull(tokens);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxLines);

        var result = new List<SyntaxToken>();
        var lines = 1;
        foreach (var token in tokens)
        {
            var end = 0;
            while (end < token.Text.Length)
            {
                if (token.Text[end] == '\n')
                {
                    if (lines == maxLines)
                    {
                        if (end > 0)
                        {
                            result.Add(token with { Text = token.Text[..end] });
                        }

                        return result;
                    }

                    lines++;
                }

                end++;
            }

            result.Add(token);
        }

        return result;
    }

    /// <summary>The number of lines in <paramref name="source"/>.</summary>
    public static int CountLines(string source)
    {
        ArgumentNullException.ThrowIfNull(source);
        return source.Count(c => c == '\n') + 1;
    }

    // Adjacent tokens of the same kind are merged so the UI creates as few spans as possible.
    private static List<SyntaxToken> Merge(IEnumerable<SyntaxToken> tokens)
    {
        var merged = new List<SyntaxToken>();
        foreach (var token in tokens)
        {
            if (token.Text.Length == 0)
            {
                continue;
            }

            if (merged.Count > 0 && merged[^1].Kind == token.Kind)
            {
                merged[^1] = merged[^1] with { Text = merged[^1].Text + token.Text };
            }
            else
            {
                merged.Add(token);
            }
        }

        return merged;
    }
}
