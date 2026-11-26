namespace MauiGallery;

/// <summary>
/// Windows 11 chrome for gallery windows: the window's Mica backdrop shows through the title bar and the docked
/// sidebar, and the pages sit on top as a solid layer, like other Windows 11 apps with a navigation pane.
/// </summary>
public partial class AppShell
{
    private readonly TitleBar _titleBar = new() { BackgroundColor = Colors.Transparent };

    partial void InitializePlatformChrome()
    {
        // MAUI's default title bar colors its text for the system theme, not the in-app theme; this one follows it.
        _titleBar.SetDynamicResource(TitleBar.ForegroundColorProperty, "ColorTextPrimary");

        // The menu bar shares the navigation bar with the page title. The page header already shows the title, so
        // the bar shows only the menus.
        SetTitleView(this, new ContentView());
    }

    partial void ApplyPlatformLayout()
    {
        if (Window is not { } window)
        {
            return;
        }

        window.TitleBar ??= _titleBar;

        var docked = FlyoutBehavior == FlyoutBehavior.Locked;

        // The docked sidebar shows the app name, like on Mac; without it, the title bar does.
        _titleBar.Title = docked ? string.Empty : window.Title ?? string.Empty;

        // Docked, the sidebar is transparent so the Mica backdrop shows through. As an overlay it needs to be solid,
        // or the page behind it would show through instead.
        if (docked)
        {
            FlyoutBackgroundColor = Colors.Transparent;
            _sidebar.BackgroundColor = Colors.Transparent;
        }
        else
        {
            SetDynamicResource(FlyoutBackgroundColorProperty, "ColorSidebar");
            _sidebar.SetDynamicResource(BackgroundColorProperty, "ColorSidebar");
        }
    }
}
