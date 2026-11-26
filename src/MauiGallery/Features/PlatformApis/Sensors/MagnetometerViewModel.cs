using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The magnetometer: the magnetic field around the device, in microteslas.</summary>
public sealed partial class MagnetometerViewModel(IMagnetometer magnetometer) : SensorViewModel(magnetometer.IsSupported)
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Reading), nameof(Strength))]
    public partial Vector3 MagneticField { get; set; }

    public string Reading => SensorText.Format(MagneticField, "0", "µT");

    /// <summary>The field's total strength. The Earth's is 25 to 65 µT; a magnet nearby pushes it far higher.</summary>
    public string Strength => $"{MagneticField.Length():0} µT";

    protected override bool IsSensorMonitoring => magnetometer.IsMonitoring;

    protected override void OnStart(SensorSpeed speed)
    {
        magnetometer.Start(speed);
        magnetometer.ReadingChanged += OnReadingChanged;
    }

    protected override void OnStop()
    {
        magnetometer.Stop();
        magnetometer.ReadingChanged -= OnReadingChanged;
    }

    private void OnReadingChanged(object? sender, MagnetometerChangedEventArgs e) =>
        OnMainThread(() => MagneticField = e.Reading.MagneticField);
}
