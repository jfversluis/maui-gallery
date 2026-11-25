namespace MauiGallery;

/// <summary>Native window setup that has no cross-platform MAUI API.</summary>
internal static class WindowChrome
{
    public static MauiAppBuilder ConfigureWindowChrome(this MauiAppBuilder builder)
    {
#if MACCATALYST
        // The sidebar already shows the app name, so hide the Mac title text, as other sidebar apps do. MAUI shows the
        // title again whenever it sets up the window content or title bar, so re-apply it after both mappings.
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow.Content), HideTitle);
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow.TitleBar), HideTitle);
#endif
        return builder;
    }

#if MACCATALYST
    private static void HideTitle(Microsoft.Maui.Handlers.IWindowHandler handler, IWindow window)
    {
        // A custom Window.TitleBar manages the title area itself.
        if (window.TitleBar is null && handler.PlatformView.WindowScene?.Titlebar is { } titlebar)
        {
            titlebar.TitleVisibility = UIKit.UITitlebarTitleVisibility.Hidden;
        }
    }
#endif
}
