using Microsoft.Maui.Platform;

namespace MauiGallery.Features.NativePower;

/// <summary>Reads the accent color the user or the system chose, with conditional compilation.</summary>
public static class SystemAccentColor
{
    /// <summary>The accent color, or <see langword="null"/> when the platform has none, and the API that was read.</summary>
    public static (Color? Color, string Source) Read()
    {
#if ANDROID
        // Material You colors arrived in Android 12; check the version at runtime too.
        if (OperatingSystem.IsAndroidVersionAtLeast(31))
        {
            var argb = Android.App.Application.Context.GetColor(Android.Resource.Color.SystemAccent1500);
            return (new Android.Graphics.Color(argb).ToColor(), "android.R.color.system_accent1_500");
        }

        return (null, "Needs Android 12 or later");
#elif IOS || MACCATALYST
        return (UIKit.UIColor.Tint.ToColor(), "UIColor.Tint");
#elif WINDOWS
        var accent = new Windows.UI.ViewManagement.UISettings().GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);
        return (accent.ToColor(), "UISettings.GetColorValue(UIColorType.Accent)");
#else
        return (null, "Not available on this platform");
#endif
    }
}
