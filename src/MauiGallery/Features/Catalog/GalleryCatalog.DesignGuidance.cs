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
            RelatedIds = ["typography", "iconography", "shape-and-elevation", "app-theme"],
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
            RelatedIds = ["color", "iconography", "spacing", "responsive-design", "fonts"],
            DocsUrl = new(DocsRoot + "user-interface/fonts"),
            SourceUrl = new(SourceRoot + "DesignGuidance"),
        },
        new()
        {
            Id = "iconography",
            Title = "Iconography",
            Description = "Fluent System Icons in two styles and four sizes, with a searchable set of common icons.",
            CategoryId = CategoryIds.DesignGuidance,
            IconGlyph = FluentIconsRegular.Icons24,
            PageType = typeof(IconographyPage),
            Keywords = ["icons", "Fluent UI System Icons", "IconFont.Maui.FluentIcons", "UseFluentIcons", "FluentIconsRegular", "FluentIconsFilled", "FontImageSource", "glyph", "icon font", "selected state", "filled", "regular"],
            RelatedIds = ["color", "typography", "shape-and-elevation", "fonts", "image-button"],
            DocsUrl = new(DocsRoot + "user-interface/fonts#display-font-icons"),
            SourceUrl = new(SourceRoot + "DesignGuidance"),
        },
        new()
        {
            Id = "spacing",
            Title = "Spacing",
            Description = "The 4-point spacing scale and the page margins that grow with the window.",
            CategoryId = CategoryIds.DesignGuidance,
            IconGlyph = FluentIconsRegular.ArrowAutofitWidth24,
            PageType = typeof(SpacingPage),
            Keywords = ["spacing", "padding", "margin", "4-point grid", "tokens", "Space16", "Thickness", "layout rhythm", "gutters"],
            RelatedIds = ["color", "typography", "shape-and-elevation", "responsive-design", "styles-and-resources"],
            DocsUrl = new(DocsRoot + "user-interface/align-position"),
            SourceUrl = new(SourceRoot + "DesignGuidance"),
        },
        new()
        {
            Id = "shape-and-elevation",
            Title = "Shape and elevation",
            Description = "Corner radii, shadows in the light theme and outlines in the dark, and cards built from Border.",
            CategoryId = CategoryIds.DesignGuidance,
            IconGlyph = FluentIconsRegular.ShapeUnion24,
            PageType = typeof(ShapeAndElevationPage),
            Keywords = ["corner radius", "rounded corners", "shadow", "elevation", "depth", "card", "Border", "StrokeShape", "RoundRectangle", "outline", "dark mode"],
            RelatedIds = ["color", "typography", "spacing", "border", "shadows"],
            DocsUrl = new(DocsRoot + "user-interface/shadow"),
            SourceUrl = new(SourceRoot + "DesignGuidance"),
        },
        new()
        {
            Id = "responsive-design",
            Title = "Responsive design",
            Description = "Compact, medium and expanded size classes, visual states that follow the window, and an adaptive Shell.",
            CategoryId = CategoryIds.DesignGuidance,
            IconGlyph = FluentIconsRegular.ResizeLarge24,
            PageType = typeof(ResponsiveDesignPage),
            Keywords = ["responsive", "adaptive", "breakpoints", "size class", "compact", "expanded", "window size", "SizeChanged", "VisualStateManager", "AdaptiveTrigger", "OnIdiom", "OnPlatform", "FlyoutBehavior", "tablet", "desktop"],
            RelatedIds = ["color", "typography", "spacing", "visual-state-manager", "shell", "two-pane-view"],
            DocsUrl = new(DocsRoot + "user-interface/visual-states"),
            SourceUrl = new(SourceRoot + "DesignGuidance"),
        },
    ];
}
