using MauiGallery.Core.Catalog;

namespace MauiGallery.Features.Catalog;

public sealed record CategorySection(GalleryCategory Category, IReadOnlyList<GalleryItem> Items);
