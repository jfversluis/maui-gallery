using System.Globalization;

namespace MauiGallery.Core.Snippets;

/// <summary>Formats option values the way they would be written in XAML or C# source.</summary>
public static class SnippetValueFormatter
{
    public static string Format(object? value) => value switch
    {
        null => string.Empty,
        bool b => b ? "True" : "False",
        double d => d.ToString("0.##", CultureInfo.InvariantCulture),
        float f => f.ToString("0.##", CultureInfo.InvariantCulture),
        IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
        _ => value.ToString() ?? string.Empty,
    };

    /// <summary>Formats a boolean as a C# literal.</summary>
    public static string FormatCSharp(object? value) => value switch
    {
        bool b => b ? "true" : "false",
        string s => $"\"{s.Replace("\"", "\\\"", StringComparison.Ordinal)}\"",
        _ => Format(value),
    };
}
