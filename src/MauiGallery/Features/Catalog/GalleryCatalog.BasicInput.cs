using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Controls.BasicInput;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] BasicInputItems() =>
    [
        new()
        {
            Id = "button",
            Title = "Button",
            Description = "Responds to a tap or click to run an action.",
            CategoryId = CategoryIds.BasicInput,
            IconGlyph = FluentIconsRegular.CursorClick24,
            PageType = typeof(ButtonPage),
            Keywords = ["click", "tap", "command", "action", "press"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIButton"),
                new(GalleryPlatforms.Android, "MaterialButton"),
                new(GalleryPlatforms.MacCatalyst, "UIButton"),
                new(GalleryPlatforms.Windows, "Button"),
            ],
            DocsUrl = new(DocsRoot + "user-interface/controls/button"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.button"),
            SourceUrl = new(SourceRoot + "Controls/BasicInput/Button"),
            IsFeatured = true,
        },
    ];
}
