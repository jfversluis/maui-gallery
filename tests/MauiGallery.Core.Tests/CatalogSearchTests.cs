using MauiGallery.Core.Search;

namespace MauiGallery.Core.Tests;

public sealed class CatalogSearchTests
{
    private static readonly CatalogSearch Search = new(
        [
            TestCatalog.Item("button", "Button", keywords: ["click", "tap"]),
            TestCatalog.Item("image-button", "ImageButton"),
            TestCatalog.Item("radio-button", "RadioButton", description: "Choose one option from a set."),
            TestCatalog.Item("entry", "Entry", categoryId: "text", description: "Single-line text input."),
            TestCatalog.Item("label", "Label", categoryId: "text", description: "Display read-only text."),
            TestCatalog.Item("date-picker", "Date picker", keywords: ["calendar"]),
        ],
        [TestCatalog.BasicInput, TestCatalog.Text]);

    private static string[] Ids(string query) => [.. Search.Search(query).Select(r => r.Item.Id)];

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void EmptyQueryReturnsNothing(string? query) => Assert.Empty(Search.Search(query));

    [Fact]
    public void ExactTitleRanksFirst() => Assert.Equal("button", Ids("button")[0]);

    [Fact]
    public void TitlePrefixRanksAboveSubstring()
    {
        var ids = Ids("button");

        Assert.Equal(["button", "image-button", "radio-button"], ids);
    }

    [Fact]
    public void KeywordsMatch() => Assert.Equal(["date-picker"], Ids("calendar"));

    [Fact]
    public void WordPrefixMatchesWithinTitle() => Assert.Equal(["date-picker"], Ids("pick"));

    [Fact]
    public void CategoryTitleMatches() => Assert.Equal(["entry", "label"], Ids("text"));

    [Fact]
    public void AllTermsMustMatch() => Assert.Equal(["entry"], Ids("text single"));

    [Fact]
    public void MatchingIgnoresCaseAndDiacritics() => Assert.Equal("entry", Ids("ÉNTRY")[0]);

    [Fact]
    public void MaxResultsLimitsOutput() => Assert.Single(Search.Search("button", maxResults: 1));
}
