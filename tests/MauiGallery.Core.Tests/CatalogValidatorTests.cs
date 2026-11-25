using MauiGallery.Core.Catalog;

namespace MauiGallery.Core.Tests;

public sealed class CatalogValidatorTests
{
    private static readonly GalleryCategory[] Categories = [TestCatalog.BasicInput, TestCatalog.Text];

    [Fact]
    public void ValidCatalogHasNoErrors()
    {
        GalleryItem[] items =
        [
            TestCatalog.Item("button", "Button") with { RelatedIds = ["image-button"] },
            TestCatalog.Item("image-button", "ImageButton"),
        ];

        Assert.Empty(CatalogValidator.Validate(Categories, items));
    }

    [Fact]
    public void DuplicateIdsAreReported()
    {
        GalleryItem[] items = [TestCatalog.Item("button", "Button"), TestCatalog.Item("button", "Button again")];

        Assert.Contains(CatalogValidator.Validate(Categories, items), e => e.Contains("Duplicate item id 'button'", StringComparison.Ordinal));
    }

    [Theory]
    [InlineData("Button")]
    [InlineData("image_button")]
    [InlineData("image button")]
    [InlineData("-button")]
    public void IdsMustBeKebabCase(string id)
    {
        Assert.Single(CatalogValidator.Validate(Categories, [TestCatalog.Item(id, "Button")]));
    }

    [Fact]
    public void UnknownCategoryIsReported()
    {
        var errors = CatalogValidator.Validate(Categories, [TestCatalog.Item("map", "Map", categoryId: "maps")]);

        Assert.Contains(errors, e => e.Contains("unknown category 'maps'", StringComparison.Ordinal));
    }

    [Fact]
    public void UnknownAndSelfRelatedItemsAreReported()
    {
        GalleryItem[] items = [TestCatalog.Item("button", "Button") with { RelatedIds = ["button", "missing"] }];

        var errors = CatalogValidator.Validate(Categories, items);

        Assert.Equal(2, errors.Count);
    }

    [Fact]
    public void ItemWithoutPlatformsIsReported()
    {
        GalleryItem[] items = [TestCatalog.Item("button", "Button") with { Platforms = GalleryPlatforms.None }];

        Assert.Single(CatalogValidator.Validate(Categories, items));
    }
}
