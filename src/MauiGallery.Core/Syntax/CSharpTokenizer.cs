using System.Collections.Frozen;

namespace MauiGallery.Core.Syntax;

internal static class CSharpTokenizer
{
    private static readonly FrozenSet<string> Keywords = FrozenSet.ToFrozenSet(
    [
        "abstract", "as", "async", "await", "base", "bool", "break", "case", "catch", "char", "class", "const",
        "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "false", "field",
        "finally", "float", "for", "foreach", "get", "if", "in", "init", "int", "interface", "internal", "is",
        "long", "namespace", "new", "null", "object", "out", "override", "partial", "private", "protected",
        "public", "readonly", "record", "ref", "required", "return", "sealed", "set", "static", "string",
        "struct", "switch", "this", "throw", "true", "try", "typeof", "using", "value", "var", "virtual", "void",
        "when", "while", "with", "yield", "nameof", "operator", "params", "global", "dynamic", "not", "and", "or",
    ], StringComparer.Ordinal);

    public static IEnumerable<SyntaxToken> Tokenize(string source)
    {
        var i = 0;
        var atLineStart = true;
        var previousWord = string.Empty;
        while (i < source.Length)
        {
            var c = source[i];
            var start = i;

            if (c == '\n')
            {
                i++;
                atLineStart = true;
                yield return new(SyntaxTokenKind.Plain, "\n");
                continue;
            }

            if (char.IsWhiteSpace(c))
            {
                while (i < source.Length && source[i] != '\n' && char.IsWhiteSpace(source[i]))
                {
                    i++;
                }

                yield return new(SyntaxTokenKind.Plain, source[start..i]);
                continue;
            }

            if (atLineStart && c == '#')
            {
                i = LineEnd(source, i);
                yield return new(SyntaxTokenKind.Preprocessor, source[start..i]);
                continue;
            }

            atLineStart = false;

            if (c == '/' && Peek(source, i + 1) == '/')
            {
                i = LineEnd(source, i);
                yield return new(SyntaxTokenKind.Comment, source[start..i]);
            }
            else if (c == '/' && Peek(source, i + 1) == '*')
            {
                var end = source.IndexOf("*/", i + 2, StringComparison.Ordinal);
                i = end < 0 ? source.Length : end + 2;
                yield return new(SyntaxTokenKind.Comment, source[start..i]);
            }
            else if (c == '"' || (c is '$' or '@' && Peek(source, i + 1) == '"'))
            {
                i = StringEnd(source, i);
                yield return new(SyntaxTokenKind.StringLiteral, source[start..i]);
            }
            else if (c == '\'')
            {
                i++;
                while (i < source.Length && source[i] != '\'' && source[i] != '\n')
                {
                    i += source[i] == '\\' ? 2 : 1;
                }

                i = Math.Min(i + 1, source.Length);
                yield return new(SyntaxTokenKind.StringLiteral, source[start..i]);
            }
            else if (char.IsDigit(c))
            {
                while (i < source.Length && (char.IsLetterOrDigit(source[i]) || source[i] is '.' or '_'))
                {
                    i++;
                }

                yield return new(SyntaxTokenKind.Number, source[start..i]);
            }
            else if (char.IsLetter(c) || c == '_')
            {
                while (i < source.Length && (char.IsLetterOrDigit(source[i]) || source[i] == '_'))
                {
                    i++;
                }

                var word = source[start..i];
                yield return new(Classify(word, previousWord, Peek(source, i)), word);
                previousWord = word;
                continue;
            }
            else
            {
                i++;
                yield return new(SyntaxTokenKind.Punctuation, source[start..i]);
            }

            previousWord = string.Empty;
        }
    }

    private static SyntaxTokenKind Classify(string word, string previousWord, char next) => word switch
    {
        _ when Keywords.Contains(word) => SyntaxTokenKind.Keyword,
        _ when previousWord == "new" => SyntaxTokenKind.TypeName,
        _ when next == '(' => SyntaxTokenKind.Method,
        _ when char.IsUpper(word[0]) => SyntaxTokenKind.TypeName,
        _ => SyntaxTokenKind.Plain,
    };

    private static int StringEnd(string source, int i)
    {
        var verbatim = false;
        while (source[i] is '$' or '@')
        {
            verbatim |= source[i] == '@';
            i++;
        }

        i++;
        while (i < source.Length)
        {
            var c = source[i];
            if (!verbatim && c == '\\')
            {
                i += 2;
                continue;
            }

            if (c == '"')
            {
                if (verbatim && Peek(source, i + 1) == '"')
                {
                    i += 2;
                    continue;
                }

                return i + 1;
            }

            if (!verbatim && c == '\n')
            {
                return i;
            }

            i++;
        }

        return source.Length;
    }

    private static int LineEnd(string source, int i)
    {
        var end = source.IndexOf('\n', i);
        return end < 0 ? source.Length : end;
    }

    private static char Peek(string source, int index) => index < source.Length ? source[index] : '\0';
}
