using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Controls.BasicInput;
using MauiGallery.Features.DesignGuidance;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private const string DocsRoot = "https://learn.microsoft.com/dotnet/maui/";
    private const string ApiRoot = "https://learn.microsoft.com/dotnet/api/";
    private const string SourceRoot = "https://github.com/jfversluis/maui-gallery/tree/main/src/MauiGallery/Features/";

    private static GalleryCategory[] CreateCategories() =>
    [
        new() { Id = CategoryIds.DesignGuidance, Title = "Design guidance", Description = "The tokens and principles behind this app's look.", IconGlyph = FluentIconsRegular.DesignIdeas24 },
        new() { Id = CategoryIds.BasicInput, Title = "Basic input", Description = "Buttons, toggles, sliders and other controls people act on.", IconGlyph = FluentIconsRegular.CursorClick24 },
        new() { Id = CategoryIds.Text, Title = "Text", Description = "Display, enter and edit text.", IconGlyph = FluentIconsRegular.TextT24 },
        new() { Id = CategoryIds.Pickers, Title = "Pickers", Description = "Choose dates, times and items from a list.", IconGlyph = FluentIconsRegular.CalendarDate24 },
        new() { Id = CategoryIds.Collections, Title = "Collections", Description = "Lists, grids and carousels of data.", IconGlyph = FluentIconsRegular.AppsList24 },
        new() { Id = CategoryIds.Layout, Title = "Layout", Description = "Arrange and size your UI.", IconGlyph = FluentIconsRegular.LayoutCellFour24 },
        new() { Id = CategoryIds.StatusAndInfo, Title = "Status and info", Description = "Progress, activity and tooltips.", IconGlyph = FluentIconsRegular.Info24 },
        new() { Id = CategoryIds.Dialogs, Title = "Dialogs and flyouts", Description = "Alerts, action sheets, prompts and modal pages.", IconGlyph = FluentIconsRegular.ChatMultiple24 },
        new() { Id = CategoryIds.Media, Title = "Media and web", Description = "Images, web content, Blazor and maps.", IconGlyph = FluentIconsRegular.Image24 },
        new() { Id = CategoryIds.Graphics, Title = "Graphics", Description = "Shapes, brushes, shadows and custom drawing.", IconGlyph = FluentIconsRegular.Shapes24 },
        new() { Id = CategoryIds.Fundamentals, Title = "Fundamentals", Description = "Data binding, styles, triggers, gestures and more.", IconGlyph = FluentIconsRegular.BookOpen24 },
        new() { Id = CategoryIds.Navigation, Title = "Navigation", Description = "Shell, tabs, flyouts, windows and deep links.", IconGlyph = FluentIconsRegular.Navigation24 },
        new() { Id = CategoryIds.PlatformApis, Title = "Platform APIs", Description = "Sensors, device info, storage and more, from one API.", IconGlyph = FluentIconsRegular.PhoneDesktop24 },
        new() { Id = CategoryIds.NativePower, Title = "Native power", Description = "Reach the platform: handlers, platform-specifics and native views.", IconGlyph = FluentIconsRegular.DeveloperBoard24 },
    ];

    private static GalleryItem[] CreateItems() =>
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
            RelatedIds = ["color"],
            DocsUrl = new(DocsRoot + "user-interface/fonts"),
            SourceUrl = new(SourceRoot + "DesignGuidance"),
        },
    ];
}
