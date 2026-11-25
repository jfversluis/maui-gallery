using System.Globalization;
using System.Text;

namespace MauiGallery.Core.Snippets;

/// <summary>Formats option values the way they would be written in XAML or C# source.</summary>
public static class SnippetValueFormatter
{
    /// <summary>Formats a value as source text, without any quoting or escaping.</summary>
    public static string Format(object? value) => value switch
    {
        null => string.Empty,
        bool b => b ? "True" : "False",
        double d => d.ToString("0.##", CultureInfo.InvariantCulture),
        float f => f.ToString("0.##", CultureInfo.InvariantCulture),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? string.Empty,
    };

    /// <summary>Formats a value for use inside a double-quoted XAML attribute.</summary>
    public static string FormatXamlAttribute(object? value)
    {
        var text = Format(value);
        var escaped = text
            .Replace("&", "&amp;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace("\"", "&quot;", StringComparison.Ordinal);

        // A leading brace would be read as a markup extension; {} escapes it.
        return value is string && escaped.StartsWith('{') ? "{}" + escaped : escaped;
    }

    /// <summary>Formats a value as a C# literal.</summary>
    public static string FormatCSharp(object? value) => value switch
    {
        null => "null",
        bool b => b ? "true" : "false",
        string s => StringLiteral(s),
        _ => Format(value),
    };

    private static string StringLiteral(string value)
    {
        var builder = new StringBuilder(value.Length + 2).Append('"');
        foreach (var c in value)
        {
            builder.Append(c switch
            {
                '"' => "\\\"",
                '\\' => "\\\\",
                '\n' => "\\n",
                '\r' => "\\r",
                '\t' => "\\t",
                _ when char.IsControl(c) => $"\\u{(int)c:x4}",
                _ => c.ToString(),
            });
        }

        return builder.Append('"').ToString();
    }
}
