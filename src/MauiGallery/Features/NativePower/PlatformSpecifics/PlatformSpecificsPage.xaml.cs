using MauiGallery.DesignSystem.Components;
using MauiGallery.Features.Samples;
using ImeFlags = Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.ImeFlags;

namespace MauiGallery.Features.NativePower;

public sealed partial class PlatformSpecificsPage : SamplePage
{
    public PlatformSpecificsPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        IosExample.NativeTarget = IosEntry;
        AndroidExample.NativeTarget = AndroidButton;
        WindowsExample.NativeTarget = WindowsButton;

        AndroidIme.ItemsSource = new[] { ImeFlags.Default, ImeFlags.Done, ImeFlags.Search, ImeFlags.Send, ImeFlags.NoPersonalizedLearning };
        AndroidIme.SelectedIndexChanged += (_, _) => AndroidImeSubstitution.Value = AndroidIme.SelectedItem;
        AndroidIme.SelectedItem = ImeFlags.NoPersonalizedLearning;

#if IOS
        // UITextField only shrinks text to fit when it has a minimum font size, and .NET MAUI doesn't set one.
        IosEntry.HandlerChanged += (_, _) =>
        {
            if (IosEntry.Handler?.PlatformView is UIKit.UITextField textField)
            {
                textField.MinimumFontSize = 10;
            }
        };
#endif

        var platform = DeviceInfo.Platform;
        Apply(IosExample, "iOS", platform == DevicePlatform.iOS);
        Apply(AndroidExample, "Android", platform == DevicePlatform.Android);
        Apply(WindowsExample, "Windows", platform == DevicePlatform.WinUI);
    }

    private static string PlatformName(DevicePlatform platform) =>
        platform == DevicePlatform.MacCatalyst ? "Mac Catalyst"
        : platform == DevicePlatform.WinUI ? "Windows"
        : platform.ToString();

    // Platform-specifics are attached properties only one platform reads; the others ignore them.
    private static void Apply(ControlExample example, string target, bool applies)
    {
        if (applies)
        {
            example.Output = $"Live: these {target} platform-specifics change the controls above";
        }
        else
        {
            example.Notice = $"{target} only: no effect on {PlatformName(DeviceInfo.Platform)}. Other platforms ignore these properties, so they're safe in shared XAML.";
        }
    }

    private void OnWindowsButtonClicked(object? sender, EventArgs e) =>
        WindowsExample.Output = DeviceInfo.Platform == DevicePlatform.WinUI
            ? "Clicked (try Alt, then the access key)"
            : "Clicked";
}
