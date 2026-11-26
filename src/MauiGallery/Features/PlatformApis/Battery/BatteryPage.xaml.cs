using System.ComponentModel;
using System.Globalization;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class BatteryPage : SamplePage
{
    private const string DesktopNote =
        "Computers report what the system's battery API returns: a desktop without a battery says Unknown, and a Mac's level can differ from the menu bar.";

    private readonly BatteryViewModel _battery;

    public BatteryPage(SamplePageServices services, BatteryViewModel battery)
        : base(services)
    {
        InitializeComponent();

        _battery = battery;
        StatusSample.BindingContext = battery;
        PlanSample.BindingContext = battery;
        battery.PropertyChanged += OnBatteryPropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _battery.Start();
        StatusExample.Notice = _battery.Problem ?? (DeviceInfo.Idiom == DeviceIdiom.Desktop ? DesktopNote : null);
    }

    protected override void OnDisappearing()
    {
        _battery.Stop();
        base.OnDisappearing();
    }

    private void OnThresholdChanged(object? sender, ValueChangedEventArgs e)
    {
        var threshold = Math.Round(e.NewValue, 2);
        _battery.LowBatteryThreshold = threshold;
        ThresholdSubstitution.Value = threshold.ToString("0.0#", CultureInfo.InvariantCulture);
    }

    private void OnBatteryPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(BatteryViewModel.LastEvent))
        {
            StatusExample.Output = _battery.LastEvent;
        }
        else if (e.PropertyName == nameof(BatteryViewModel.SyncPlan))
        {
            PlanExample.Output = $"SyncPlan: {_battery.SyncPlan}";
        }
    }
}
