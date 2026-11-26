using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;

namespace MauiGallery.Features.NativePower;

/// <summary>
/// An attached property that tints an <see cref="Entry"/>'s caret, selection or underline through native APIs, which
/// .NET MAUI has no cross-platform property for. Only entries that set it are customized.
/// </summary>
public static class NativeTint
{
    public static readonly BindableProperty ColorProperty =
        BindableProperty.CreateAttached("Color", typeof(Color), typeof(NativeTint), null, propertyChanged: OnColorChanged);

    public static Color? GetColor(BindableObject view) => (Color?)view.GetValue(ColorProperty);

    public static void SetColor(BindableObject view, Color? value) => view.SetValue(ColorProperty, value);

    // The mapping key isn't a property of the control, so ask the handler to run it again.
    private static void OnColorChanged(BindableObject bindable, object? oldValue, object? newValue) =>
        (bindable as IView)?.Handler?.UpdateValue(nameof(NativeTint));

    internal static void Customize() =>
        EntryHandler.Mapper.AppendToMapping(nameof(NativeTint), (handler, view) =>
        {
            if (view is not Entry entry || !entry.IsSet(ColorProperty))
            {
                return;
            }

            // A null color restores the platform default.
            var color = GetColor(entry);
#if IOS || MACCATALYST
            handler.PlatformView.TintColor = color?.ToPlatform();
#elif ANDROID
            var tint = color is null ? null : Android.Content.Res.ColorStateList.ValueOf(color.ToPlatform());
            handler.PlatformView.BackgroundTintList = tint;
            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {
                handler.PlatformView.TextCursorDrawable?.SetTintList(tint);
            }
#elif WINDOWS
            if (color is null)
            {
                handler.PlatformView.ClearValue(Microsoft.UI.Xaml.Controls.TextBox.SelectionHighlightColorProperty);
            }
            else
            {
                handler.PlatformView.SelectionHighlightColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(color.ToWindowsColor());
            }
#endif
        });
}
