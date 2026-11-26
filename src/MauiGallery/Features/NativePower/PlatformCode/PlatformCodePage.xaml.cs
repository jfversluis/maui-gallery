using MauiGallery.Features.Samples;

namespace MauiGallery.Features.NativePower;

public sealed partial class PlatformCodePage : SamplePage
{
    private readonly INativeOsInfo _osInfo;

    public PlatformCodePage(SamplePageServices services, INativeOsInfo osInfo)
        : base(services)
    {
        _osInfo = osInfo;
        InitializeComponent();
        RuntimeExample.NativeTarget = RuntimeGreeting;
        AccentExample.NativeTarget = AccentHex;
        PartialExample.NativeTarget = OsBuild;

        ShowRuntimeInfo();
        ReadAccent();
        ReadOsInfo();
    }

    private void ShowRuntimeInfo()
    {
        var platform = DeviceInfo.Platform;
        RuntimePlatform.Text = platform.ToString();
        RuntimeIdiom.Text = DeviceInfo.Idiom.ToString();
        RuntimeVersion.Text = DeviceInfo.VersionString;

        // Each platform has its own version check; the others return false.
        (RuntimeCheckName.Text, RuntimeCheck.Text) =
            platform == DevicePlatform.Android ? ("Android 12+", YesNo(OperatingSystem.IsAndroidVersionAtLeast(31)))
            : platform == DevicePlatform.iOS ? ("iOS 26+", YesNo(OperatingSystem.IsIOSVersionAtLeast(26)))
            : platform == DevicePlatform.MacCatalyst ? ("Mac Catalyst 26+", YesNo(OperatingSystem.IsMacCatalystVersionAtLeast(26)))
            : ("Windows 11", YesNo(OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000)));
    }

    private static string YesNo(bool value) => value ? "Yes" : "No";

    private void ReadAccent()
    {
        var (color, source) = SystemAccentColor.Read();
        AccentSwatch.BackgroundColor = color ?? Colors.Transparent;
        AccentHex.Text = color?.ToArgbHex() ?? "No accent color";
        AccentExample.Output = $"Read {source}";
    }

    private void ReadOsInfo()
    {
        OsBuild.Text = _osInfo.GetBuild();
        OsThermal.Text = _osInfo.GetThermalState() ?? "Not reported on this platform";
    }

    private void OnReadAccentClicked(object? sender, EventArgs e) => ReadAccent();

    private void OnRefreshOsInfoClicked(object? sender, EventArgs e)
    {
        ReadOsInfo();
        PartialExample.Output = $"Refreshed at {DateTime.Now:T}";
    }
}
