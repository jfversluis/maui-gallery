using MauiGallery.Core.Catalog;

namespace MauiGallery.Features.Home;

public sealed record CategoryTile(GalleryCategory Category, int ItemCount)
{
    public string CountText => ItemCount == 1 ? "1 page" : $"{ItemCount} pages";
}
