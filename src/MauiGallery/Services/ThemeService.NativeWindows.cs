#if IOS || MACCATALYST
using UIKit;

namespace MauiGallery.Services;

/// <summary>
/// On iOS and Mac Catalyst, MAUI applies <see cref="Application.UserAppTheme"/> only to the current view controller,
/// so other windows, and windows opened later, keep the system style. Set it on every native window instead.
/// </summary>
public sealed partial class ThemeService
{
    partial void RegisterNativeWindowStyle() =>
        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(ThemeService), (handler, _) => ApplyTo(handler.PlatformView));

    partial void ApplyNativeWindowStyle()
    {
        foreach (var window in _application?.Windows ?? [])
        {
            if (window.Handler?.PlatformView is UIWindow nativeWindow)
            {
                ApplyTo(nativeWindow);
            }
        }
    }

    private void ApplyTo(UIWindow window) => window.OverrideUserInterfaceStyle = _application?.UserAppTheme switch
    {
        AppTheme.Light => UIUserInterfaceStyle.Light,
        AppTheme.Dark => UIUserInterfaceStyle.Dark,
        _ => UIUserInterfaceStyle.Unspecified,
    };
}
#endif
