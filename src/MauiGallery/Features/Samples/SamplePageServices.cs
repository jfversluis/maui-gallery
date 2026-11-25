using MauiGallery.Features.Catalog;
using MauiGallery.Services;

namespace MauiGallery.Features.Samples;

/// <summary>The services every sample page needs, bundled so sample pages have a one-parameter constructor.</summary>
public sealed record SamplePageServices(
    GalleryCatalog Catalog,
    IFavoritesService Favorites,
    IRecentsService Recents,
    INavigationService Navigation,
    IBrowser Browser,
    IClipboard Clipboard);
