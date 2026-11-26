namespace MauiGallery.DesignSystem.Components;

/// <summary>
/// An <see cref="Entry"/> without its platform border or underline, for chrome that draws its own frame (such as
/// the search box). Only this subclass is affected; regular entries keep their native look.
/// </summary>
public sealed class BorderlessEntry : Entry
{
    private static bool s_registered;

    /// <summary>Registers the handler customization. Safe to call more than once.</summary>
    public static void Register()
    {
        if (s_registered)
        {
            return;
        }

        s_registered = true;
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
        {
            if (view is not BorderlessEntry)
            {
                return;
            }

#if IOS || MACCATALYST
            handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#elif ANDROID
            handler.PlatformView.Background = null;
#elif WINDOWS
            // The TextBox template swaps in its own border, underline and background on hover and focus, so override
            // those theme resources too, and drop the side padding the surrounding frame already provides.
            var textBox = handler.PlatformView;
            textBox.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            textBox.Background = null;
            textBox.Padding = new Microsoft.UI.Xaml.Thickness(0, 5, 0, 6);
            var transparent = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Transparent);
            textBox.Resources["TextControlBorderThemeThicknessFocused"] = new Microsoft.UI.Xaml.Thickness(0);
            textBox.Resources["TextControlBackgroundPointerOver"] = transparent;
            textBox.Resources["TextControlBackgroundFocused"] = transparent;
#endif
        });
    }
}
