using System.Collections.Frozen;

namespace MauiGallery.Core.Catalog;

/// <summary>
/// The pages that show their first example as captured on every platform ("Same code, every platform"). The images
/// live in <c>Resources/Raw/platform-shots/&lt;id&gt;/&lt;platform&gt;-&lt;theme&gt;.png</c> and are captured with DevFlow
/// by the scripts in <c>eng/platform-shots</c>, which read the ids from this file.
/// </summary>
public static class PlatformShots
{
    /// <summary>The folder, relative to the app package, that holds the images.</summary>
    public const string Folder = "platform-shots";

    /// <summary>Pages whose subject is a single native control. Keep one id per line: the capture scripts parse it.</summary>
    public static readonly FrozenSet<string> ItemIds = FrozenSet.Create(
        StringComparer.Ordinal,
        [
            "button",
            "image-button",
            "checkbox",
            "radio-button",
            "switch",
            "slider",
            "stepper",
            "label",
            "entry",
            "editor",
            "search-bar",
            "picker",
            "date-picker",
            "time-picker",
            "collection-view",
            "carousel-view",
            "indicator-view",
            "refresh-view",
            "activity-indicator",
            "progress-bar",
            "border",
            "scroll-view",
            "image",
            "shapes",
            "brushes",
            "shadows",
            "graphics-view",
            "paths",
        ]);

    /// <summary>The platforms in the order the tiles show them.</summary>
    public static IReadOnlyList<GalleryPlatforms> Platforms { get; } =
        [GalleryPlatforms.iOS, GalleryPlatforms.Android, GalleryPlatforms.MacCatalyst, GalleryPlatforms.Windows];

    public static bool Has(string itemId) => ItemIds.Contains(itemId);

    /// <summary>The package-relative path of one image, for example <c>platform-shots/button/ios-dark.png</c>.</summary>
    public static string PathFor(string itemId, GalleryPlatforms platform, bool dark) =>
        $"{Folder}/{itemId}/{FileNameFor(platform, dark)}";

    public static string FileNameFor(GalleryPlatforms platform, bool dark) =>
        $"{SlugFor(platform)}-{(dark ? "dark" : "light")}.png";

    public static string SlugFor(GalleryPlatforms platform) => platform switch
    {
        GalleryPlatforms.iOS => "ios",
        GalleryPlatforms.Android => "android",
        GalleryPlatforms.MacCatalyst => "maccatalyst",
        GalleryPlatforms.Windows => "windows",
        _ => throw new ArgumentOutOfRangeException(nameof(platform), platform, "Platform shots exist for a single platform only."),
    };

    /// <summary>Reads the pixel size from a PNG's header, so an image can be shown at its captured size.</summary>
    public static bool TryReadPngSize(ReadOnlySpan<byte> png, out int width, out int height)
    {
        ReadOnlySpan<byte> signature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        width = height = 0;
        if (png.Length < 24 || !png[..8].SequenceEqual(signature) || !png.Slice(12, 4).SequenceEqual("IHDR"u8))
        {
            return false;
        }

        width = System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(png.Slice(16, 4));
        height = System.Buffers.Binary.BinaryPrimitives.ReadInt32BigEndian(png.Slice(20, 4));
        return width > 0 && height > 0;
    }
}
