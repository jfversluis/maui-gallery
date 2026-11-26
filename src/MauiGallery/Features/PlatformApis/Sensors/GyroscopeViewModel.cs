using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The gyroscope: how fast the device turns around each axis, in radians per second.</summary>
public sealed partial class GyroscopeViewModel(IGyroscope gyroscope) : SensorViewModel(gyroscope.IsSupported)
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Reading))]
    public partial Vector3 AngularVelocity { get; set; }

    public string Reading => SensorText.Format(AngularVelocity, "0.00", "rad/s");

    protected override bool IsSensorMonitoring => gyroscope.IsMonitoring;

    protected override void OnStart(SensorSpeed speed)
    {
        gyroscope.Start(speed);
        gyroscope.ReadingChanged += OnReadingChanged;
    }

    protected override void OnStop()
    {
        gyroscope.Stop();
        gyroscope.ReadingChanged -= OnReadingChanged;
    }

    private void OnReadingChanged(object? sender, GyroscopeChangedEventArgs e) =>
        OnMainThread(() => AngularVelocity = e.Reading.AngularVelocity);
}
