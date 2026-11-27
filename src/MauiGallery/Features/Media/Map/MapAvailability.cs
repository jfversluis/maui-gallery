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
    public static MapAvailabilityState Check()
    {
        var hasApiKey = true;
        CheckApiKey(ref hasApiKey);
        return hasApiKey ? MapAvailabilityState.Available : MapAvailabilityState.MissingApiKey;
    }

    // Implemented in Platforms/Android and Platforms/Windows only: Apple Maps needs no key, so on iOS and Mac the call
    // compiles away.
    static partial void CheckApiKey(ref bool hasApiKey);
}
