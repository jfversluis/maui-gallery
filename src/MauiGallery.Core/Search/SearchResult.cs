using MauiGallery.Core.Catalog;

namespace MauiGallery.Core.Search;

/// <summary>A gallery item that matched a search query, with its relevance score.</summary>
public sealed record SearchResult(GalleryItem Item, int Score);
