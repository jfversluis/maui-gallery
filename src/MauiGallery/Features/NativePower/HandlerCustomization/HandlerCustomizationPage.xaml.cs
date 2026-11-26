using System.ComponentModel;
using System.Globalization;
using MauiGallery.DesignSystem.Components;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.NativePower;

public sealed partial class HandlerCustomizationPage : SamplePage
{
    public HandlerCustomizationPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        SubclassExample.NativeTarget = PlainSample;
        TintExample.NativeTarget = TintSample;
        HapticExample.NativeTarget = HapticSample;

        HapticSample.Ticked += (_, step) =>
            HapticExample.Output = DeviceInfo.Platform == DevicePlatform.WinUI
                ? string.Create(CultureInfo.InvariantCulture, $"Crossed {step:0}: Windows has no slider haptics")
                : string.Create(CultureInfo.InvariantCulture, $"Crossed {step:0}: played the selection haptic");

        ApplyTint();
    }

    /// <summary>The native property the customization sets on this platform.</summary>
    private static string TintTarget =>
        DeviceInfo.Platform == DevicePlatform.Android ? "EditText.BackgroundTintList and TextCursorDrawable"
        : DeviceInfo.Platform == DevicePlatform.WinUI ? "TextBox.SelectionHighlightColor"
        : "UITextField.TintColor";

    private void OnTintChanged(object? sender, ToggledEventArgs e) => ApplyTint();

    private void OnTintColorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SwatchPicker.SelectedColor))
        {
            ApplyTint();
        }
    }

    private void ApplyTint()
    {
        var color = TintEnabled.IsToggled ? TintColor.SelectedColor : null;
        NativeTint.SetColor(TintSample, color);
        TintSubstitution.Value = color;
        TintExample.Output = color is null
            ? $"UpdateValue(\"NativeTint\") restored the default {TintTarget}"
            : $"UpdateValue(\"NativeTint\") set {TintTarget} to {color.ToArgbHex()}";
    }
}
