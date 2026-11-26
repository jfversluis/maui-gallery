using System.Globalization;

namespace MauiGallery.Features.PlatformApis;

/// <summary>Formats a <see cref="Location"/> for the location samples.</summary>
public static class LocationText
{
    public static string Coordinates(Location location) =>
        string.Create(CultureInfo.InvariantCulture, $"{location.Latitude:0.00000}, {location.Longitude:0.00000}");

    public static string Details(Location location)
    {
        var accuracy = location.Accuracy is { } meters ? $"±{meters:0} m" : "Accuracy unknown";
        var age = DateTimeOffset.UtcNow - location.Timestamp;
        var when = age.TotalSeconds < 5 ? "just now"
            : age.TotalMinutes < 1 ? $"{age.TotalSeconds:0} s ago"
            : age.TotalHours < 1 ? $"{age.TotalMinutes:0} min ago"
            : location.Timestamp.ToLocalTime().ToString("g", CultureInfo.CurrentCulture);
        var source = location.IsFromMockProvider ? " · from a mock provider" : string.Empty;
        return $"{accuracy} · {when}{source}";
    }
}
