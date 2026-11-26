using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Navigation;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] NavigationItems() =>
    [
        new()
        {
            Id = "shell",
            Title = "Shell",
            Description = "Structures an app with tabs and a flyout, and navigates between pages with URI routes.",
            CategoryId = CategoryIds.Navigation,
            IconGlyph = FluentIconsRegular.PanelLeftText24,
            PageType = typeof(ShellSamplePage),
            Keywords = ["shell", "GoToAsync", "route", "routing", "RegisterRoute", "query", "IQueryAttributable", "parameters", "back", "tabs", "flyout", "sidebar", "NavBarIsVisible", "TabBarIsVisible", "FlyoutBehavior", "navigation"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UITabBarController + UINavigationController"),
                new(GalleryPlatforms.Android, "BottomNavigationView + DrawerLayout"),
                new(GalleryPlatforms.MacCatalyst, "UINavigationController + flyout view"),
                new(GalleryPlatforms.Windows, "NavigationView"),
            ],
            RelatedIds = ["navigation-page", "tabbed-page", "flyout-page", "deep-links", "modal-pages"],
            DocsUrl = new(DocsRoot + "fundamentals/shell/"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.shell"),
            SourceUrl = new(SourceRoot + "Navigation/Shell"),
            IsFeatured = true,
            IsUpdated = true,
        },
        new()
        {
            Id = "tabbed-page",
            Title = "Tabbed page",
            Description = "Shows a set of pages as tabs, with icons, colors and badges, for apps that don't use Shell.",
            CategoryId = CategoryIds.Navigation,
            IconGlyph = FluentIconsRegular.Tabs24,
            PageType = typeof(TabbedPageSamplePage),
            Keywords = ["TabbedPage", "tabs", "tab bar", "bottom navigation", "BadgeText", "badge", "SelectedTabColor", "ToolbarPlacement", "CurrentPage"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UITabBarController"),
                new(GalleryPlatforms.Android, "TabLayout or BottomNavigationView"),
                new(GalleryPlatforms.MacCatalyst, "UITabBarController"),
                new(GalleryPlatforms.Windows, "NavigationView (top)"),
            ],
            RelatedIds = ["shell", "navigation-page", "flyout-page", "modal-pages"],
            DocsUrl = new(DocsRoot + "user-interface/pages/tabbedpage"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.tabbedpage"),
            SourceUrl = new(SourceRoot + "Navigation/TabbedPage"),
            IsUpdated = true,
        },
        new()
        {
            Id = "flyout-page",
            Title = "Flyout page",
            Description = "Pairs a menu page with a detail page, shown as an overlay or side by side.",
            CategoryId = CategoryIds.Navigation,
            IconGlyph = FluentIconsRegular.PanelLeftExpand24,
            PageType = typeof(FlyoutPageSamplePage),
            Keywords = ["FlyoutPage", "flyout", "hamburger", "menu", "drawer", "master detail", "split view", "IsPresented", "FlyoutLayoutBehavior"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIViewController containment"),
                new(GalleryPlatforms.Android, "DrawerLayout"),
                new(GalleryPlatforms.MacCatalyst, "UIViewController containment"),
                new(GalleryPlatforms.Windows, "NavigationView (left)"),
            ],
            RelatedIds = ["shell", "navigation-page", "tabbed-page"],
            DocsUrl = new(DocsRoot + "user-interface/pages/flyoutpage"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.flyoutpage"),
            SourceUrl = new(SourceRoot + "Navigation/FlyoutPage"),
        },
        new()
        {
            Id = "navigation-page",
            Title = "Navigation page",
            Description = "Keeps a stack of pages with a navigation bar and back button, for apps that don't use Shell.",
            CategoryId = CategoryIds.Navigation,
            IconGlyph = FluentIconsRegular.StackArrowForward24,
            PageType = typeof(NavigationPageSamplePage),
            Keywords = ["NavigationPage", "PushAsync", "PopAsync", "PopToRootAsync", "stack", "back button", "navigation bar", "TitleView", "BackButtonTitle", "HasNavigationBar"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UINavigationController"),
                new(GalleryPlatforms.Android, "Fragment back stack + MaterialToolbar"),
                new(GalleryPlatforms.MacCatalyst, "UINavigationController"),
                new(GalleryPlatforms.Windows, "Frame + title bar"),
            ],
            RelatedIds = ["shell", "modal-pages", "toolbar-items", "flyout-page"],
            DocsUrl = new(DocsRoot + "user-interface/pages/navigationpage"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.navigationpage"),
            SourceUrl = new(SourceRoot + "Navigation/NavigationPage"),
        },
        new()
        {
            Id = "multi-window",
            Title = "Multiple windows",
            Description = "Opens, sizes and closes extra windows, and follows each window's lifecycle.",
            CategoryId = CategoryIds.Navigation,
            IconGlyph = FluentIconsRegular.WindowMultiple24,
            PageType = typeof(MultiWindowPage),
            Platforms = GalleryPlatforms.iOS | GalleryPlatforms.MacCatalyst | GalleryPlatforms.Windows,
            Keywords = ["window", "multi-window", "OpenWindow", "CloseWindow", "ActivateWindow", "scene", "iPad", "desktop", "lifecycle", "Activated", "Destroying", "Width", "Height", "MinimumWidth"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIWindowScene (iPad)"),
                new(GalleryPlatforms.MacCatalyst, "UIWindowScene"),
                new(GalleryPlatforms.Windows, "Microsoft.UI.Xaml.Window"),
            ],
            RelatedIds = ["title-bar", "menu-bar", "keyboard-accelerators", "shell"],
            DocsUrl = new(DocsRoot + "fundamentals/windows"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.application.openwindow"),
            SourceUrl = new(SourceRoot + "Navigation/MultiWindow"),
        },
        new()
        {
            Id = "title-bar",
            Title = "Title bar",
            Description = "Draws a desktop window's title bar with a title, an icon and your own controls, such as search.",
            CategoryId = CategoryIds.Navigation,
            IconGlyph = FluentIconsRegular.TabDesktop24,
            PageType = typeof(TitleBarPage),
            Platforms = GalleryPlatforms.MacCatalyst | GalleryPlatforms.Windows,
            Keywords = ["TitleBar", "title bar", "window", "caption", "Subtitle", "LeadingContent", "TrailingContent", "search", "desktop", "chrome"],
            NativeMappings =
            [
                new(GalleryPlatforms.MacCatalyst, "UITitlebar (hidden) + MAUI view"),
                new(GalleryPlatforms.Windows, "Custom title bar (ExtendsContentIntoTitleBar)"),
            ],
            RelatedIds = ["multi-window", "menu-bar", "search-bar"],
            DocsUrl = new(DocsRoot + "user-interface/controls/titlebar"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.titlebar"),
            SourceUrl = new(SourceRoot + "Navigation/TitleBar"),
        },
        new()
        {
            Id = "deep-links",
            Title = "Deep links",
            Description = "Opens a specific page from a link, such as mauigallery://item/button, on every platform.",
            CategoryId = CategoryIds.Navigation,
            IconGlyph = FluentIconsRegular.LinkSquare24,
            PageType = typeof(DeepLinksPage),
            Platforms = GalleryPlatforms.Android | GalleryPlatforms.iOS | GalleryPlatforms.MacCatalyst,
            Keywords = ["deep link", "URI scheme", "URL scheme", "Launcher", "OpenAsync", "CFBundleURLTypes", "intent filter", "SceneOpenUrl", "OnNewIntent", "App Links", "Universal Links"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "CFBundleURLTypes + UISceneDelegate"),
                new(GalleryPlatforms.Android, "Intent filter (ACTION_VIEW)"),
                new(GalleryPlatforms.MacCatalyst, "CFBundleURLTypes + UISceneDelegate"),
            ],
            RelatedIds = ["shell", "multi-window"],
            DocsUrl = new(DocsRoot + "platform-integration/appmodel/launcher"),
            ApiUrl = new(ApiRoot + "microsoft.maui.applicationmodel.launcher.openasync"),
            SourceUrl = new(SourceRoot + "Navigation/DeepLinks"),
        },
    ];
}
