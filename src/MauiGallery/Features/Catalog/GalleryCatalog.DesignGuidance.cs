using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.DesignGuidance;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] DesignGuidanceItems() =>
    [
        new()
        {
            Id = "color",
            Title = "Color",
            Description = "The palette, semantic color tokens and how they adapt to light and dark themes.",
            CategoryId = CategoryIds.DesignGuidance,
            IconGlyph = FluentIconsRegular.Color24,
            PageType = typeof(ColorPage),
            Keywords = ["theme", "dark mode", "light mode", "palette", "tokens", "AppThemeBinding", "DynamicResource"],
            RelatedIds = ["typography"],
            DocsUrl = new(DocsRoot + "user-interface/theming"),
            SourceUrl = new(SourceRoot + "DesignGuidance"),
        },
        new()
        {
            Id = "typography",
            Title = "Typography",
            Description = "The type ramp, custom fonts and font scaling.",
            CategoryId = CategoryIds.DesignGuidance,
            IconGlyph = FluentIconsRegular.TextFont24,
            PageType = typeof(TypographyPage),
            Keywords = ["fonts", "text styles", "Inter", "type ramp", "font size"],
            RelatedIds = ["color", "label"],
            DocsUrl = new(DocsRoot + "user-interface/fonts"),
            SourceUrl = new(SourceRoot + "DesignGuidance"),
        },
    ];
}
