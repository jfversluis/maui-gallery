using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The compass: the heading of the top of the device, in degrees from magnetic north.</summary>
public sealed partial class CompassViewModel(ICompass compass) : SensorViewModel(compass.IsSupported)
{
    private static readonly string[] Directions = ["N", "NE", "E", "SE", "S", "SW", "W", "NW"];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Reading))]
    public partial double? Heading { get; set; }

    /// <summary>Smooths the readings on Android. The other platforms ignore it.</summary>
    [ObservableProperty]
    public partial bool ApplyLowPassFilter { get; set; }

    public string Reading => Heading is { } heading ? $"{heading:0}° {Directions[(int)Math.Round(heading / 45) % 8]}" : "–";

    protected override bool IsSensorMonitoring => compass.IsMonitoring;

    protected override void OnStart(SensorSpeed speed)
    {
        compass.Start(speed, ApplyLowPassFilter);
        compass.ReadingChanged += OnReadingChanged;
    }

    protected override void OnStop()
    {
        compass.Stop();
        compass.ReadingChanged -= OnReadingChanged;
    }

    // The filter is chosen when the compass starts.
    partial void OnApplyLowPassFilterChanged(bool value) => Restart();

    private void OnReadingChanged(object? sender, CompassChangedEventArgs e) =>
        OnMainThread(() => Heading = e.Reading.HeadingMagneticNorth);
}
