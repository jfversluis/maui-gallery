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
