using MauiGallery.Core.Catalog;

namespace MauiGallery.Features.Samples;

public sealed record NativeMappingDisplay(string Platform, string NativeType)
{
    public static NativeMappingDisplay From(NativeMapping mapping) => new(PlatformName(mapping.Platform), mapping.NativeType);

    /// <summary>The name readers know the platform by: Mac Catalyst apps run on macOS.</summary>
    public static string PlatformName(GalleryPlatforms platform) => platform switch
    {
        GalleryPlatforms.iOS => "iOS",
        GalleryPlatforms.Android => "Android",
        GalleryPlatforms.MacCatalyst => "macOS",
        GalleryPlatforms.Windows => "Windows",
        _ => platform.ToString(),
    };

    public string Text => $"{Platform} · {NativeType}";
}
