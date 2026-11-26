using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The barometer: the air pressure, in hectopascals.</summary>
public sealed partial class BarometerViewModel(IBarometer barometer) : SensorViewModel(barometer.IsSupported)
{
    private const double SeaLevelPressure = 1013.25;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Reading), nameof(Altitude))]
    public partial double? Pressure { get; set; }

    public string Reading => Pressure is { } pressure ? $"{pressure:0.0} hPa" : "–";

    // The international barometric formula: roughly 8 m higher for every hectopascal less.
    public string Altitude => Pressure is { } pressure
        ? $"About {44330 * (1 - Math.Pow(pressure / SeaLevelPressure, 1 / 5.255)):0} m above sea level, on a standard day"
        : "Start to read the air pressure";

    protected override bool IsSensorMonitoring => barometer.IsMonitoring;

    protected override void OnStart(SensorSpeed speed)
    {
        barometer.Start(speed);
        barometer.ReadingChanged += OnReadingChanged;
    }

    protected override void OnStop()
    {
        barometer.Stop();
        barometer.ReadingChanged -= OnReadingChanged;
    }

    private void OnReadingChanged(object? sender, BarometerChangedEventArgs e) =>
        OnMainThread(() => Pressure = e.Reading.PressureInHectopascals);
}
