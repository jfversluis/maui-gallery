namespace MauiGallery.DesignSystem.Components;

/// <summary>
/// An <see cref="Entry"/> without its platform border or underline, for chrome that draws its own frame (such as
/// the search box). Only this subclass is affected; regular entries keep their native look.
/// </summary>
public sealed class BorderlessEntry : Entry
{
    /// <summary>Registers the handler customization. Call once from <c>MauiProgram</c>.</summary>
    public static void Register() =>
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
            handler.PlatformView.BorderThickness = new Microsoft.UI.Xaml.Thickness(0);
            handler.PlatformView.Background = null;
#endif
        });
}
