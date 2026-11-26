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
        PlatformTitle.Text = $"This device runs {PlatformName(platform)}";
        IosExample.Output = Applicability("iOS", platform == DevicePlatform.iOS);
        AndroidExample.Output = Applicability("Android", platform == DevicePlatform.Android);
        WindowsExample.Output = Applicability("Windows", platform == DevicePlatform.WinUI);
    }

    private static string PlatformName(DevicePlatform platform) =>
        platform == DevicePlatform.MacCatalyst ? "Mac Catalyst"
        : platform == DevicePlatform.WinUI ? "Windows"
        : platform.ToString();

    private static string Applicability(string target, bool applies) =>
        applies
            ? $"Live: these {target} platform-specifics change the controls above"
            : $"Ignored on {PlatformName(DeviceInfo.Platform)}: only {target} reads these properties";

    private void OnWindowsButtonClicked(object? sender, EventArgs e) =>
        WindowsExample.Output = DeviceInfo.Platform == DevicePlatform.WinUI
            ? "Clicked (try Alt, then the access key)"
            : "Clicked";
}
