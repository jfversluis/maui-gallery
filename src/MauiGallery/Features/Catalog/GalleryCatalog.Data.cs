using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;

namespace MauiGallery.Features.Catalog;

/// <summary>
/// Categories, plus the list of per-category item files. Each category's items live in
/// <c>GalleryCatalog.&lt;Category&gt;.cs</c> so pages can be added without touching a shared list.
/// </summary>
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
        new() { Id = CategoryIds.MenusAndToolbars, Title = "Menus and toolbars", Description = "Menu bars, toolbar items, context menus and keyboard shortcuts.", IconGlyph = FluentIconsRegular.MoreHorizontal24 },
        new() { Id = CategoryIds.Media, Title = "Media and web", Description = "Images, web content, Blazor and maps.", IconGlyph = FluentIconsRegular.Image24 },
        new() { Id = CategoryIds.Graphics, Title = "Graphics", Description = "Shapes, brushes, shadows and custom drawing.", IconGlyph = FluentIconsRegular.Shapes24 },
        new() { Id = CategoryIds.Fundamentals, Title = "Fundamentals", Description = "Data binding, styles, triggers, gestures and more.", IconGlyph = FluentIconsRegular.BookOpen24 },
        new() { Id = CategoryIds.Navigation, Title = "Navigation", Description = "Shell, tabs, flyouts, windows and deep links.", IconGlyph = FluentIconsRegular.Navigation24 },
        new() { Id = CategoryIds.PlatformApis, Title = "Platform APIs", Description = "Sensors, device info, storage and more, from one API.", IconGlyph = FluentIconsRegular.PhoneDesktop24 },
        new() { Id = CategoryIds.NativePower, Title = "Native power", Description = "Reach the platform: handlers, platform-specifics and native views.", IconGlyph = FluentIconsRegular.DeveloperBoard24 },
    ];

    private static GalleryItem[] CreateItems() =>
    [
        .. DesignGuidanceItems(),
        .. BasicInputItems(),
        .. TextItems(),
        .. PickersItems(),
        .. CollectionsItems(),
        .. LayoutItems(),
        .. StatusAndInfoItems(),
        .. DialogsItems(),
        .. MenusAndToolbarsItems(),
        .. MediaItems(),
        .. GraphicsItems(),
        .. FundamentalsItems(),
        .. FundamentalsBindingItems(),
        .. NavigationItems(),
        .. PlatformApisItems(),
        .. PlatformApisCoreItems(),
        .. NativePowerItems(),
    ];
}
