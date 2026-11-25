using MauiGallery.Core.Catalog;

namespace MauiGallery.Core.Tests;

internal static class TestCatalog
{
    public static readonly GalleryCategory BasicInput = new()
    {
        Id = "basic-input",
        Title = "Basic input",
        Description = "Buttons, toggles and sliders.",
        IconGlyph = "B",
    };

    public static readonly GalleryCategory Text = new()
    {
        Id = "text",
        Title = "Text",
        Description = "Display and edit text.",
        IconGlyph = "T",
    };

    public static GalleryItem Item(string id, string title, string categoryId = "basic-input", string description = "A control.", params string[] keywords) => new()
    {
        Id = id,
        Title = title,
        Description = description,
        CategoryId = categoryId,
        IconGlyph = "I",
        PageType = typeof(object),
        Keywords = keywords,
    };
}
