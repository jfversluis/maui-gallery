using System.Globalization;
using System.Numerics;

namespace MauiGallery.Features.PlatformApis;

/// <summary>Formats sensor readings for the sensor samples.</summary>
public static class SensorText
{
    public static string Format(Vector3 value, string format, string unit) =>
        $"X {Number(value.X, format)}   Y {Number(value.Y, format)}   Z {Number(value.Z, format)} {unit}";

    // Formats a reading without the "-0.00" that tiny negative values otherwise show.
    public static string Number(double value, string format)
    {
        var text = value.ToString(format, CultureInfo.CurrentCulture);
        return text.TrimStart('-').All(c => c is '0' or '.' or ',') ? text.TrimStart('-') : text;
    }
}
