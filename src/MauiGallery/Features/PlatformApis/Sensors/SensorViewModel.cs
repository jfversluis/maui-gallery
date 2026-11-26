using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>
/// What the sensor samples share: an IsSupported check, a SensorSpeed, and starting and stopping. Each sensor
/// subscribes to its own reading event in <see cref="OnStart"/>.
/// </summary>
/// <remarks>
/// A sensor is shared by the whole app, so every window's view model talks to the same one. Only the view model that
/// started it stops it; the others say it's in use.
/// </remarks>
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

    /// <summary>Whether the sensor is running, started by this view model or by another window's.</summary>
    protected abstract bool IsSensorMonitoring { get; }

    /// <summary>Starts the sensor, then subscribes: if starting throws, there's nothing to undo.</summary>
    protected abstract void OnStart(SensorSpeed speed);

    protected abstract void OnStop();

    // Readings can arrive on a background thread; the UI may only change on the main thread.
    protected static void OnMainThread(Action action) => MainThread.BeginInvokeOnMainThread(action);

    /// <summary>Restarts a running sensor, to apply a setting that only takes effect when it starts.</summary>
    protected void Restart()
    {
        if (IsMonitoring)
        {
            Stop();
            Start();
        }
    }

    private void Start()
    {
        if (IsSensorMonitoring)
        {
            Message = "Another window is using this sensor. Stop it there first.";
            return;
        }

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
        catch (Exception ex)
        {
            // For example a platform permission error. The sensor didn't start, so there's nothing to undo.
            Message = $"The sensor didn't start: {ex.Message}";
        }
    }

    // A new speed only takes effect when the sensor starts.
    partial void OnSpeedIndexChanged(int value) => Restart();
}
