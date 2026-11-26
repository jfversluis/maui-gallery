using CommunityToolkit.Mvvm.ComponentModel;
using MauiGallery.Core.Catalog;

namespace MauiGallery.Features.Samples;

/// <summary>One platform's screenshot in the "Same code, every platform" section.</summary>
public sealed partial class PlatformShotTile(string itemTitle, GalleryPlatforms platform, string platformName, string? nativeType, bool isCurrent) : ObservableObject
{
    public GalleryPlatforms Platform { get; } = platform;

    public string PlatformName { get; } = platformName;

    public string NativeType { get; } = nativeType ?? string.Empty;

    public bool HasNativeType => NativeType.Length > 0;

    /// <summary>True for the platform the app is running on, which the tile highlights.</summary>
    public bool IsCurrent { get; } = isCurrent;

    [ObservableProperty]
    public partial ImageSource? Image { get; set; }

    /// <summary>The screenshot's size in device-independent units, as the platform drew the control.</summary>
    public Size NaturalSize { get; set; }

    /// <summary>The size the screenshot is shown at: its natural size, scaled down when the tile is too small.</summary>
    [ObservableProperty]
    public partial double DisplayWidth { get; set; }

    [ObservableProperty]
    public partial double DisplayHeight { get; set; }

    /// <summary>The height of the image area, shared by every tile so the captions line up.</summary>
    [ObservableProperty]
    public partial double WellHeight { get; set; }

    [ObservableProperty]
    public partial bool IsMissing { get; set; }

    /// <summary>What a screen reader says for the tile, which is a single accessible element.</summary>
    public string Description { get; } = $"{itemTitle} on {platformName}{(isCurrent ? ", the platform you're using," : string.Empty)} rendered as {(string.IsNullOrEmpty(nativeType) ? "the native control" : nativeType)}";
}
