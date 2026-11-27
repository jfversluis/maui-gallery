using System.Reflection;

namespace MauiGallery.Features.Media;

public static partial class MapAvailability
{
    /// <summary>
    /// The Azure Maps key, compiled in from the AzureMapsKey build property (see MauiGallery.csproj); null when unset.
    /// </summary>
    internal static string? AzureMapsKey { get; } = typeof(MapAvailability).Assembly
        .GetCustomAttributes<AssemblyMetadataAttribute>()
        .FirstOrDefault(attribute => attribute.Key == "AzureMapsKey")?.Value;

    // The build-time key (registered with UseMapServiceToken at startup) or one pasted on the Map page.
    static partial void CheckApiKey(ref bool hasApiKey) =>
        hasApiKey = !string.IsNullOrWhiteSpace(Microsoft.Maui.ApplicationModel.Platform.MapServiceToken);

    static partial void SetRuntimeKey(string key) => Microsoft.Maui.ApplicationModel.Platform.MapServiceToken = key;
}
