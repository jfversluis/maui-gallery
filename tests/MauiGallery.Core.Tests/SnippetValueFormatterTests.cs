using System.Globalization;
using MauiGallery.Core.Snippets;

namespace MauiGallery.Core.Tests;

public sealed class SnippetValueFormatterTests
{
    [Fact]
    public void FormatsXamlValuesInvariantly()
    {
        var previous = CultureInfo.CurrentCulture;
        CultureInfo.CurrentCulture = new CultureInfo("nl-NL");
        try
        {
            Assert.Equal("1.5", SnippetValueFormatter.Format(1.5));
            Assert.Equal("True", SnippetValueFormatter.Format(true));
            Assert.Equal(string.Empty, SnippetValueFormatter.Format(null));
        }
        finally
        {
            CultureInfo.CurrentCulture = previous;
        }
    }

    [Fact]
    public void FormatsCSharpLiterals()
    {
        Assert.Equal("false", SnippetValueFormatter.FormatCSharp(false));
        Assert.Equal("\"Say \\\"hi\\\"\"", SnippetValueFormatter.FormatCSharp("Say \"hi\""));
        Assert.Equal("12", SnippetValueFormatter.FormatCSharp(12d));
    }

    [Theory]
    [InlineData("Fish & Chips", "Fish &amp; Chips")]
    [InlineData("Say \"hi\"", "Say &quot;hi&quot;")]
    [InlineData("a < b", "a &lt; b")]
    [InlineData("{curly}", "{}{curly}")]
    public void EscapesXamlAttributes(string value, string expected) => Assert.Equal(expected, SnippetValueFormatter.FormatXamlAttribute(value));

    [Fact]
    public void EscapesCSharpStrings()
    {
        Assert.Equal("\"C:\\\\new\"", SnippetValueFormatter.FormatCSharp("C:\\new"));
        Assert.Equal("\"a\\nb\"", SnippetValueFormatter.FormatCSharp("a\nb"));
        Assert.Equal("null", SnippetValueFormatter.FormatCSharp(null));
    }
}
