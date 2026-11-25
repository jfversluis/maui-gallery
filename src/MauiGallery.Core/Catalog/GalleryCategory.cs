namespace MauiGallery.Core.Catalog;

/// <summary>A group of related gallery items, shown as a section in navigation.</summary>
public sealed record GalleryCategory
{
    public required string Id { get; init; }

    public required string Title { get; init; }

    public required string Description { get; init; }

    /// <summary>The icon font glyph shown next to the category.</summary>
    public required string IconGlyph { get; init; }
}
