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

    static partial void CheckApiKey(ref bool hasApiKey) => hasApiKey = !string.IsNullOrWhiteSpace(AzureMapsKey);
}
