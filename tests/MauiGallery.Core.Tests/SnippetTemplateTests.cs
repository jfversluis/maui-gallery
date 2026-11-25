using MauiGallery.Core.Snippets;
using MauiGallery.Core.Syntax;

namespace MauiGallery.Core.Tests;

public sealed class SnippetTemplateTests
{
    [Fact]
    public void PlaceholdersAreDiscoveredInOrder()
    {
        var template = new SnippetTemplate("""<Button Text="$(Text)" IsEnabled="$(IsEnabled)" Command="$(Text)" />""", CodeLanguage.Xaml);

        Assert.Equal(["Text", "IsEnabled"], template.Placeholders);
    }

    [Fact]
    public void ValuesAreSubstituted()
    {
        var template = new SnippetTemplate("""<Button Text="$(Text)" />""", CodeLanguage.Xaml);

        Assert.Equal("""<Button Text="Save" />""", template.Render(new Dictionary<string, string?> { ["Text"] = "Save" }));
    }

    [Fact]
    public void NullValueRemovesLineAndKeepsElementWellFormed()
    {
        var template = new SnippetTemplate("""
            <Button
                Text="$(Text)"
                IsEnabled="$(IsEnabled)" />
            """, CodeLanguage.Xaml);

        var rendered = template.Render(new Dictionary<string, string?> { ["Text"] = "Save", ["IsEnabled"] = null });

        Assert.Equal("""
            <Button
                Text="Save" />
            """.ReplaceLineEndings("\n"), rendered);
    }

    [Fact]
    public void NullValueOnOpeningTagLineMovesClosingBracket()
    {
        var template = new SnippetTemplate("""
            <Border
                Padding="$(Padding)">
                <Label />
            </Border>
            """, CodeLanguage.Xaml);

        var rendered = template.Render(new Dictionary<string, string?> { ["Padding"] = null });

        Assert.StartsWith("<Border>\n", rendered, StringComparison.Ordinal);
    }

    [Fact]
    public void CSharpLinesAreRemovedWithoutCarryingTerminators()
    {
        var template = new SnippetTemplate("""
            var button = new Button();
            button.IsEnabled = $(IsEnabled);
            """, CodeLanguage.CSharp);

        Assert.Equal("var button = new Button();", template.Render(new Dictionary<string, string?> { ["IsEnabled"] = null }));
    }

    [Fact]
    public void MissingValueThrows()
    {
        var template = new SnippetTemplate("""<Button Text="$(Text)" />""", CodeLanguage.Xaml);

        Assert.Throws<KeyNotFoundException>(() => template.Render(new Dictionary<string, string?>()));
    }

    [Fact]
    public void LineEndingsAreNormalized()
    {
        var template = new SnippetTemplate("a\r\nb\r\n", CodeLanguage.Xaml);

        Assert.Equal("a\nb", template.Render(new Dictionary<string, string?>()));
    }

    [Fact]
    public void CSharpNeverCarriesTerminators()
    {
        var template = new SnippetTemplate("""
            var answer =
                $(Factory) =>
                42;
            """, CodeLanguage.CSharp);

        Assert.Equal("var answer =\n    42;", template.Render(new Dictionary<string, string?> { ["Factory"] = null }));
    }
}
