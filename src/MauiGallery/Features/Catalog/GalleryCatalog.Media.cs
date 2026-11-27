using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Media;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] MediaItems() =>
    [
        new()
        {
            Id = "map",
            Title = "Map",
            Description = "Shows an interactive Apple, Google or Azure map with pins, routes, areas and the user's location.",
            CategoryId = CategoryIds.Media,
            IconGlyph = FluentIconsRegular.Map24,
            PageType = typeof(MapPage),
            Keywords = ["Map", "maps", "Microsoft.Maui.Controls.Maps", "UseMauiMaps", "MapType", "MapSpan", "MoveToRegion", "VisibleRegion", "Region", "Pin", "MarkerClicked", "InfoWindowClicked", "MapClicked", "IsShowingUser", "UserLocationChanged", "MapElements", "Polyline", "Polygon", "Circle", "Distance", "Apple Maps", "Google Maps", "Azure Maps", "API key", "UseMapServiceToken", "MapControl", "MapKit", "MKMapView"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "MKMapView"),
                new(GalleryPlatforms.Android, "MapView (Google Maps)"),
                new(GalleryPlatforms.MacCatalyst, "MKMapView"),
                new(GalleryPlatforms.Windows, "MapControl (Azure Maps)"),
            ],
            RelatedIds = ["geolocation", "geocoding", "permissions"],
            DocsUrl = new(DocsRoot + "user-interface/controls/map"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.maps.map"),
            SourceUrl = new(SourceRoot + "Media/Map"),
            IsUpdated = true,
        },
    ];
}
