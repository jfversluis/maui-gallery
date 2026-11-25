using System.Diagnostics.CodeAnalysis;

namespace MauiGallery.Core.Catalog;

/// <summary>A single page in the gallery, such as a control, API or design guidance topic.</summary>
public sealed record GalleryItem
{
    /// <summary>A stable, URL-safe identifier. Also used as the navigation route and deep-link segment.</summary>
    public required string Id { get; init; }

    public required string Title { get; init; }

    /// <summary>A one-sentence summary shown on cards and in search results.</summary>
    public required string Description { get; init; }

    public required string CategoryId { get; init; }

    /// <summary>The icon font glyph shown on cards and in navigation.</summary>
    public required string IconGlyph { get; init; }

    /// <summary>The page that renders this item. Resolved through dependency injection.</summary>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public required Type PageType { get; init; }

    public IReadOnlyList<string> Keywords { get; init; } = [];

    public GalleryPlatforms Platforms { get; init; } = GalleryPlatforms.All;

    public IReadOnlyList<NativeMapping> NativeMappings { get; init; } = [];

    public IReadOnlyList<string> RelatedIds { get; init; } = [];

    public Uri? DocsUrl { get; init; }

    public Uri? ApiUrl { get; init; }

    public Uri? SourceUrl { get; init; }

    public bool IsNew { get; init; }

    public bool IsUpdated { get; init; }

    /// <summary>Featured items are highlighted on the home page.</summary>
    public bool IsFeatured { get; init; }
}
