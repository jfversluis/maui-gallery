using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The orientation sensor: how the device is rotated relative to the Earth, as a quaternion.</summary>
public sealed partial class OrientationViewModel(IOrientationSensor orientation) : SensorViewModel(orientation.IsSupported)
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Reading), nameof(TiltForward), nameof(TiltSideways), nameof(Angles))]
    public partial Quaternion Orientation { get; set; } = Quaternion.Identity;

    public string Reading =>
        $"X {Number(Orientation.X)}   Y {Number(Orientation.Y)}   Z {Number(Orientation.Z)}   W {Number(Orientation.W)}";

    /// <summary>Rotation around the device's X axis, in degrees: positive when the top edge lifts.</summary>
    public double TiltForward
    {
        get
        {
            var q = Orientation;
            return ToDegrees(Math.Atan2(2 * ((q.W * q.X) + (q.Y * q.Z)), 1 - (2 * ((q.X * q.X) + (q.Y * q.Y)))));
        }
    }

    /// <summary>Rotation around the device's Y axis, in degrees: positive when the left edge lifts.</summary>
    public double TiltSideways
    {
        get
        {
            var q = Orientation;
            return ToDegrees(Math.Asin(Math.Clamp(2 * ((q.W * q.Y) - (q.Z * q.X)), -1, 1)));
        }
    }

    public string Angles => $"Forward {SensorText.Number(TiltForward, "0")}°   Sideways {SensorText.Number(TiltSideways, "0")}°";

    protected override void OnStart(SensorSpeed speed)
    {
        orientation.ReadingChanged += OnReadingChanged;
        orientation.Start(speed);
    }

    protected override void OnStop()
    {
        orientation.Stop();
        orientation.ReadingChanged -= OnReadingChanged;
    }

    private static string Number(float value) => SensorText.Number(value, "0.00");

    private static double ToDegrees(double radians) => radians * 180 / Math.PI;

    private void OnReadingChanged(object? sender, OrientationSensorChangedEventArgs e) =>
        OnMainThread(() => Orientation = e.Reading.Orientation);
}
