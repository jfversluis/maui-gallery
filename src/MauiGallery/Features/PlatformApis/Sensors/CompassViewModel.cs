using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The compass: the heading of the top of the device, in degrees from magnetic north.</summary>
public sealed partial class CompassViewModel(ICompass compass) : SensorViewModel(compass.IsSupported)
{
    private static readonly string[] Directions = ["N", "NE", "E", "SE", "S", "SW", "W", "NW"];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Reading))]
    public partial double Heading { get; set; }

    /// <summary>Smooths the readings on Android. The other platforms ignore it.</summary>
    [ObservableProperty]
    public partial bool ApplyLowPassFilter { get; set; }

    public string Reading => $"{Heading:0}° {Directions[(int)Math.Round(Heading / 45) % 8]}";

    protected override void OnStart(SensorSpeed speed)
    {
        compass.ReadingChanged += OnReadingChanged;
        compass.Start(speed, ApplyLowPassFilter);
    }

    protected override void OnStop()
    {
        compass.Stop();
        compass.ReadingChanged -= OnReadingChanged;
    }

    // The filter is chosen when the compass starts, so restart it.
    partial void OnApplyLowPassFilterChanged(bool value)
    {
        if (IsMonitoring)
        {
            OnStop();
            OnStart((SensorSpeed)SpeedIndex);
        }
    }

    private void OnReadingChanged(object? sender, CompassChangedEventArgs e) =>
        OnMainThread(() => Heading = e.Reading.HeadingMagneticNorth);
}
