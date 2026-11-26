using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The barometer: the air pressure, in hectopascals.</summary>
public sealed partial class BarometerViewModel(IBarometer barometer) : SensorViewModel(barometer.IsSupported)
{
    private const double SeaLevelPressure = 1013.25;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Reading), nameof(Altitude))]
    public partial double Pressure { get; set; } = SeaLevelPressure;

    public string Reading => $"{Pressure:0.0} hPa";

    // The international barometric formula: roughly 8 m higher for every hectopascal less.
    public string Altitude => $"About {44330 * (1 - Math.Pow(Pressure / SeaLevelPressure, 1 / 5.255)):0} m above sea level, on a standard day";

    protected override void OnStart(SensorSpeed speed)
    {
        barometer.ReadingChanged += OnReadingChanged;
        barometer.Start(speed);
    }

    protected override void OnStop()
    {
        barometer.Stop();
        barometer.ReadingChanged -= OnReadingChanged;
    }

    private void OnReadingChanged(object? sender, BarometerChangedEventArgs e) =>
        OnMainThread(() => Pressure = e.Reading.PressureInHectopascals);
}
