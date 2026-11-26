namespace MauiGallery.Features.Media;

public enum MapAvailabilityState
{
    Available,

    /// <summary>.NET MAUI Maps has no Windows implementation.</summary>
    NotSupported,

    /// <summary>Android needs a Google Maps API key in the manifest, and this build has none.</summary>
    MissingApiKey,
}

/// <summary>Whether the map control can render in this app, on this platform.</summary>
public static partial class MapAvailability
{
    public static MapAvailabilityState Check()
    {
        if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            return MapAvailabilityState.NotSupported;
        }

        var hasApiKey = true;
        CheckApiKey(ref hasApiKey);
        return hasApiKey ? MapAvailabilityState.Available : MapAvailabilityState.MissingApiKey;
    }

    // Implemented in Platforms/Android only: Apple Maps needs no key, so elsewhere the call compiles away.
    static partial void CheckApiKey(ref bool hasApiKey);
}
