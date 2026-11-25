using MauiGallery.Core.Catalog;

namespace MauiGallery.Features.Samples;

public sealed record NativeMappingDisplay(string Platform, string NativeType)
{
    public static NativeMappingDisplay From(NativeMapping mapping) => new(
        mapping.Platform switch
        {
            GalleryPlatforms.iOS => "iOS",
            GalleryPlatforms.Android => "Android",
            GalleryPlatforms.MacCatalyst => "macOS",
            GalleryPlatforms.Windows => "Windows",
            _ => mapping.Platform.ToString(),
        },
        mapping.NativeType);

    public string Text => $"{Platform} · {NativeType}";
}
