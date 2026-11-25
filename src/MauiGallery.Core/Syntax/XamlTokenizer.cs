namespace MauiGallery.Core.Syntax;

internal static class XamlTokenizer
{
    public static IEnumerable<SyntaxToken> Tokenize(string source)
    {
        var i = 0;
        while (i < source.Length)
        {
            if (source.AsSpan(i).StartsWith("<!--", StringComparison.Ordinal))
            {
                var end = source.IndexOf("-->", i + 4, StringComparison.Ordinal);
                end = end < 0 ? source.Length : end + 3;
                yield return new(SyntaxTokenKind.Comment, source[i..end]);
                i = end;
            }
            else if (source[i] == '<')
            {
                foreach (var token in TokenizeTag(source, ref i))
                {
                    yield return token;
                }
            }
            else
            {
                var end = source.IndexOf('<', i);
                end = end < 0 ? source.Length : end;
                yield return new(SyntaxTokenKind.Plain, source[i..end]);
                i = end;
            }
        }
    }

    private static List<SyntaxToken> TokenizeTag(string source, ref int i)
    {
        var tokens = new List<SyntaxToken>();
        var start = i;
        i++;
        if (i < source.Length && source[i] is '/' or '?')
        {
            i++;
        }

        tokens.Add(new(SyntaxTokenKind.Punctuation, source[start..i]));
        tokens.Add(new(SyntaxTokenKind.ElementName, ReadName(source, ref i)));

        while (i < source.Length)
        {
            var c = source[i];
            if (char.IsWhiteSpace(c))
            {
                var wsStart = i;
                while (i < source.Length && char.IsWhiteSpace(source[i]))
                {
                    i++;
                }

                tokens.Add(new(SyntaxTokenKind.Plain, source[wsStart..i]));
            }
            else if (c == '>')
            {
                tokens.Add(new(SyntaxTokenKind.Punctuation, ">"));
                i++;
                break;
            }
            else if (c is '/' or '?' && i + 1 < source.Length && source[i + 1] == '>')
            {
                tokens.Add(new(SyntaxTokenKind.Punctuation, source.Substring(i, 2)));
                i += 2;
                break;
            }
            else if (c == '=')
            {
                tokens.Add(new(SyntaxTokenKind.Punctuation, "="));
                i++;
            }
            else if (c is '"' or '\'')
            {
                var end = source.IndexOf(c, i + 1);
                end = end < 0 ? source.Length : end + 1;
                var value = source[i..end];
                var inner = value.Trim(c);
                tokens.Add(new(
                    inner.StartsWith('{') && !inner.StartsWith("{}", StringComparison.Ordinal)
                        ? SyntaxTokenKind.MarkupExtension
                        : SyntaxTokenKind.AttributeValue,
                    value));
                i = end;
            }
            else
            {
                var name = ReadName(source, ref i);
                if (name.Length == 0)
                {
                    tokens.Add(new(SyntaxTokenKind.Plain, source[i].ToString()));
                    i++;
                }
                else
                {
                    tokens.Add(new(SyntaxTokenKind.AttributeName, name));
                }
            }
        }

        return tokens;
    }

    private static string ReadName(string source, ref int i)
    {
        var start = i;
        while (i < source.Length && (char.IsLetterOrDigit(source[i]) || source[i] is ':' or '.' or '_' or '-'))
        {
            i++;
        }

        return source[start..i];
    }
}
