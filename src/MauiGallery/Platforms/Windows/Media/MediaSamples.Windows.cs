namespace MauiGallery.Features.Media;

public static partial class MediaSamples
{
    // WinUI's MapControl renders Azure Maps and reads its key from the Essentials map service token.
    static partial void ConfigureMaps(MauiAppBuilder builder)
    {
        if (MapAvailability.AzureMapsKey is { Length: > 0 } key)
        {
            builder.ConfigureEssentials(essentials => essentials.UseMapServiceToken(key));
        }
    }
}
