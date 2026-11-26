using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>
/// What the sensor samples share: an IsSupported check, a SensorSpeed, and starting and stopping. Each sensor
/// subscribes to its own reading event in <see cref="OnStart"/>.
/// </summary>
public abstract partial class SensorViewModel(bool isSupported) : ObservableObject
{
    public bool IsSupported { get; } = isSupported;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ToggleText))]
    public partial bool IsMonitoring { get; set; }

    public string ToggleText => IsMonitoring ? "Stop" : "Start";

    /// <summary>The <see cref="SensorSpeed"/> as an index into Default, UI, Game, Fastest.</summary>
    [ObservableProperty]
    public partial int SpeedIndex { get; set; } = (int)SensorSpeed.UI;

    [ObservableProperty]
    public partial string? Message { get; set; } = isSupported ? null : "This device doesn't have this sensor.";

    [RelayCommand(CanExecute = nameof(IsSupported))]
    private void Toggle()
    {
        if (IsMonitoring)
        {
            Stop();
        }
        else
        {
            Start();
        }
    }

    /// <summary>Stops the sensor. Call it when the page disappears: a running sensor drains the battery.</summary>
    public void Stop()
    {
        if (IsMonitoring)
        {
            OnStop();
            IsMonitoring = false;
        }
    }

    protected abstract void OnStart(SensorSpeed speed);

    protected abstract void OnStop();

    // Readings can arrive on a background thread; the UI may only change on the main thread.
    protected static void OnMainThread(Action action) => MainThread.BeginInvokeOnMainThread(action);

    private void Start()
    {
        try
        {
            OnStart((SensorSpeed)SpeedIndex);
            IsMonitoring = true;
            Message = null;
        }
        catch (FeatureNotSupportedException)
        {
            Message = "This device doesn't have this sensor.";
        }
    }

    // A new speed only takes effect when the sensor starts, so restart it.
    partial void OnSpeedIndexChanged(int value)
    {
        if (IsMonitoring)
        {
            Stop();
            Start();
        }
    }
}
