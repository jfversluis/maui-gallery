using System.ComponentModel;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class AppAndDeviceInfoPage : SamplePage
{
    private readonly DisplayInfoViewModel _display;

    public AppAndDeviceInfoPage(
        SamplePageServices services,
        AppInfoViewModel appInfo,
        DeviceInfoViewModel deviceInfo,
        DisplayInfoViewModel display)
        : base(services)
    {
        InitializeComponent();

        AppSample.BindingContext = appInfo;
        AppExample.NativeTarget = AppSettingsButton;
        DeviceSample.BindingContext = deviceInfo;

        _display = display;
        DisplaySample.BindingContext = display;
        DisplayOptions.BindingContext = display;
        display.PropertyChanged += OnDisplayPropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _display.Start();
    }

    protected override void OnDisappearing()
    {
        _display.Stop();
        base.OnDisappearing();
    }

    private void OnShowSettingsClicked(object? sender, EventArgs e) =>
        AppExample.Output = "ShowSettingsUI() opened this app's settings";

    private void OnDisplayPropertyChanged(object? sender, PropertyChangedEventArgs e) =>
        DisplayExample.Output = e.PropertyName switch
        {
            nameof(DisplayInfoViewModel.Info) => $"MainDisplayInfoChanged: {_display.SizeInPixels}, {_display.Info.Orientation}",
            nameof(DisplayInfoViewModel.KeepScreenOn) => $"KeepScreenOn = {_display.KeepScreenOn}",
            _ => DisplayExample.Output,
        };
}
