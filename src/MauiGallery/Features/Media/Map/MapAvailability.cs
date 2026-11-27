namespace MauiGallery.Features.Media;

public enum MapAvailabilityState
{
    Available,

    /// <summary>
    /// The platform's map service needs a key this build doesn't have: Google Maps on Android, Azure Maps on Windows.
    /// </summary>
    MissingApiKey,
}

/// <summary>Whether the map control can render in this app, on this platform.</summary>
public static partial class MapAvailability
{
    /// <summary>
    /// Whether a key can be supplied while the app runs. Windows reads it from <c>Platform.MapServiceToken</c> each
    /// time it creates a map; Google Maps on Android only reads its key from the app manifest.
    /// </summary>
    public static bool AcceptsRuntimeKey => DeviceInfo.Platform == DevicePlatform.WinUI;

    public static MapAvailabilityState Check()
    {
        var hasApiKey = true;
        CheckApiKey(ref hasApiKey);
        return hasApiKey ? MapAvailabilityState.Available : MapAvailabilityState.MissingApiKey;
    }

    /// <summary>Uses <paramref name="key"/> for maps created from now on, until the app closes.</summary>
    public static void UseRuntimeKey(string key) => SetRuntimeKey(key);

    // Implemented in Platforms/Android and Platforms/Windows only: Apple Maps needs no key, so on iOS and Mac the calls
    // compile away.
    static partial void CheckApiKey(ref bool hasApiKey);

    static partial void SetRuntimeKey(string key);
}
