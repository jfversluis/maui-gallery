using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The accelerometer, in multiples of gravity (g), with shake detection.</summary>
public sealed partial class AccelerometerViewModel(IAccelerometer accelerometer) : SensorViewModel(accelerometer.IsSupported)
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Reading))]
    public partial Vector3 Acceleration { get; set; } = new(0, 0, 1);

    [ObservableProperty]
    public partial int ShakeCount { get; set; }

    public string Reading => SensorText.Format(Acceleration, "0.00", "g");

    protected override bool IsSensorMonitoring => accelerometer.IsMonitoring;

    protected override void OnStart(SensorSpeed speed)
    {
        accelerometer.Start(speed);
        accelerometer.ReadingChanged += OnReadingChanged;
        accelerometer.ShakeDetected += OnShakeDetected;
    }

    protected override void OnStop()
    {
        accelerometer.Stop();
        accelerometer.ReadingChanged -= OnReadingChanged;
        accelerometer.ShakeDetected -= OnShakeDetected;
    }

    private void OnReadingChanged(object? sender, AccelerometerChangedEventArgs e) =>
        OnMainThread(() => Acceleration = e.Reading.Acceleration);

    private void OnShakeDetected(object? sender, EventArgs e) => OnMainThread(() => ShakeCount++);
}
