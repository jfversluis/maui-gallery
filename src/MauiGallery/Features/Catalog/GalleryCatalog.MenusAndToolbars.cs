using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Controls.Menus;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] MenusAndToolbarsItems() =>
    [
        new()
        {
            Id = "menu-bar",
            Title = "Menu bar",
            Description = "Adds menus to the desktop menu bar, app-wide on the Shell or per page while it's shown.",
            CategoryId = CategoryIds.MenusAndToolbars,
            IconGlyph = FluentIconsRegular.WindowText24,
            PageType = typeof(MenuBarPage),
            Keywords = ["menu", "menu bar", "MenuBarItem", "MenuFlyoutItem", "MenuFlyoutSubItem", "separator", "desktop", "mac", "windows", "submenu"],
            Platforms = GalleryPlatforms.MacCatalyst | GalleryPlatforms.Windows,
            NativeMappings =
            [
                new(GalleryPlatforms.MacCatalyst, "UIMenuBuilder + UIMenu"),
                new(GalleryPlatforms.Windows, "MenuBar"),
            ],
            RelatedIds = ["keyboard-accelerators", "context-menu", "toolbar-items"],
            DocsUrl = new(DocsRoot + "user-interface/menu-bar"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.menubaritem"),
            SourceUrl = new(SourceRoot + "Controls/Menus/MenuBar"),
            IsFeatured = true,
        },
        new()
        {
            Id = "toolbar-items",
            Title = "Toolbar items",
            Description = "Puts actions in the page's navigation bar, with less common ones in an overflow menu.",
            CategoryId = CategoryIds.MenusAndToolbars,
            IconGlyph = FluentIconsRegular.MoreHorizontal24,
            PageType = typeof(ToolbarItemsPage),
            Keywords = ["toolbar", "ToolbarItem", "navigation bar", "app bar", "action", "overflow", "secondary", "primary", "icon"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIBarButtonItem"),
                new(GalleryPlatforms.Android, "MaterialToolbar menu item"),
                new(GalleryPlatforms.MacCatalyst, "UIBarButtonItem"),
                new(GalleryPlatforms.Windows, "AppBarButton"),
            ],
            RelatedIds = ["menu-bar", "context-menu"],
            DocsUrl = new(DocsRoot + "user-interface/toolbaritem"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.toolbaritem"),
            SourceUrl = new(SourceRoot + "Controls/Menus/ToolbarItems"),
        },
        new()
        {
            Id = "context-menu",
            Title = "Context menu",
            Description = "Shows actions for a view when the user right-clicks it.",
            CategoryId = CategoryIds.MenusAndToolbars,
            IconGlyph = FluentIconsRegular.MoreVertical24,
            PageType = typeof(ContextMenuPage),
            Keywords = ["context menu", "right-click", "ContextFlyout", "MenuFlyout", "FlyoutBase", "long press", "secondary click", "submenu"],
            Platforms = GalleryPlatforms.MacCatalyst | GalleryPlatforms.Windows,
            NativeMappings =
            [
                new(GalleryPlatforms.MacCatalyst, "UIContextMenuInteraction"),
                new(GalleryPlatforms.Windows, "MenuFlyout"),
            ],
            RelatedIds = ["menu-bar", "action-sheets", "swipe-view"],
            DocsUrl = new(DocsRoot + "user-interface/context-menu"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.flyoutbase.contextflyoutproperty"),
            SourceUrl = new(SourceRoot + "Controls/Menus/ContextMenu"),
        },
        new()
        {
            Id = "keyboard-accelerators",
            Title = "Keyboard accelerators",
            Description = "Gives menu items keyboard shortcuts, with the right modifier on each platform.",
            CategoryId = CategoryIds.MenusAndToolbars,
            IconGlyph = FluentIconsRegular.KeyCommand24,
            PageType = typeof(KeyboardAcceleratorsPage),
            Keywords = ["keyboard", "shortcut", "accelerator", "KeyboardAccelerator", "hotkey", "command", "ctrl", "cmd", "modifier"],
            Platforms = GalleryPlatforms.MacCatalyst | GalleryPlatforms.Windows,
            NativeMappings =
            [
                new(GalleryPlatforms.MacCatalyst, "UIKeyCommand"),
                new(GalleryPlatforms.Windows, "KeyboardAccelerator"),
            ],
            RelatedIds = ["menu-bar", "context-menu"],
            DocsUrl = new(DocsRoot + "user-interface/keyboard-accelerators"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.keyboardaccelerator"),
            SourceUrl = new(SourceRoot + "Controls/Menus/KeyboardAccelerators"),
        },
    ];
}
