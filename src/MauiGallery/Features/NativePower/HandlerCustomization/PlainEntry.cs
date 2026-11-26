using Microsoft.Maui.Handlers;

namespace MauiGallery.Features.NativePower;

/// <summary>
/// An <see cref="Entry"/> without the platform's border or underline. The handler customization checks for this
/// subclass, so every other <see cref="Entry"/> in the app keeps its native look.
/// </summary>
public sealed class PlainEntry : Entry
{
    internal static void Customize() =>
        EntryHandler.Mapper.AppendToMapping(nameof(PlainEntry), (handler, view) =>
        {
            if (view is not PlainEntry)
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
