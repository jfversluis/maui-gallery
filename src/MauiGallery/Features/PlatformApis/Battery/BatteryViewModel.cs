using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the Battery samples.</summary>
public sealed partial class BatteryViewModel(IBattery battery) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ChargeText), nameof(HasChargeLevel), nameof(SyncPlan))]
    public partial double ChargeLevel { get; set; } = -1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SyncPlan))]
    public partial BatteryState State { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SyncPlan))]
    public partial BatteryPowerSource PowerSource { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SyncPlan))]
    public partial EnergySaverStatus EnergySaver { get; set; }

    /// <summary>Below this charge level, and not charging, background work pauses.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SyncPlan))]
    public partial double LowBatteryThreshold { get; set; } = 0.2;

    /// <summary>Why the battery can't be read here, or <see langword="null"/> when it can.</summary>
    [ObservableProperty]
    public partial string? Problem { get; set; }

    /// <summary>The last change event, for the sample's output.</summary>
    [ObservableProperty]
    public partial string? LastEvent { get; set; }

    // ChargeLevel is -1 where the device doesn't report it, such as a simulator or a desktop without a battery.
    public bool HasChargeLevel => ChargeLevel >= 0;

    public string ChargeText => HasChargeLevel ? $"{ChargeLevel:P0}" : "Not reported";

    public string SyncPlan =>
        EnergySaver == EnergySaverStatus.On ? "Paused: energy saver is on"
        : PowerSource is BatteryPowerSource.AC or BatteryPowerSource.Usb or BatteryPowerSource.Wireless ? "Every 5 minutes, plugged in"
        : HasChargeLevel && ChargeLevel < LowBatteryThreshold ? "Paused: the battery is low"
        : "Every 30 minutes";

    /// <summary>Reads the battery and starts listening for changes. Call it when the page appears.</summary>
    public void Start()
    {
        try
        {
            Read();
            battery.BatteryInfoChanged += OnBatteryInfoChanged;
            battery.EnergySaverStatusChanged += OnEnergySaverStatusChanged;
            Problem = null;
        }
        catch (PermissionException)
        {
            // Android needs the BATTERY_STATS permission in AndroidManifest.xml.
            Problem = "Declare the BATTERY_STATS permission to read the battery.";
        }
        catch (FeatureNotSupportedException)
        {
            Problem = "This device doesn't report its battery.";
        }
    }

    /// <summary>Stops listening. Call it when the page disappears.</summary>
    public void Stop()
    {
        if (Problem is null)
        {
            battery.BatteryInfoChanged -= OnBatteryInfoChanged;
            battery.EnergySaverStatusChanged -= OnEnergySaverStatusChanged;
        }
    }

    private void Read()
    {
        ChargeLevel = battery.ChargeLevel;
        State = battery.State;
        PowerSource = battery.PowerSource;
        EnergySaver = battery.EnergySaverStatus;
    }

    private void OnBatteryInfoChanged(object? sender, BatteryInfoChangedEventArgs e)
    {
        ChargeLevel = e.ChargeLevel;
        State = e.State;
        PowerSource = e.PowerSource;
        LastEvent = $"BatteryInfoChanged: {ChargeText}, {State}, {PowerSource}";
    }

    private void OnEnergySaverStatusChanged(object? sender, EnergySaverStatusChangedEventArgs e)
    {
        EnergySaver = e.EnergySaverStatus;
        LastEvent = $"EnergySaverStatusChanged: {EnergySaver}";
    }
}
