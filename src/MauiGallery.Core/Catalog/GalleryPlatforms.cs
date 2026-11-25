namespace MauiGallery.Core.Catalog;

/// <summary>The platforms a gallery sample runs on.</summary>
[Flags]
public enum GalleryPlatforms
{
    None = 0,
    Android = 1 << 0,
    iOS = 1 << 1,
    MacCatalyst = 1 << 2,
    Windows = 1 << 3,
    All = Android | iOS | MacCatalyst | Windows,
}
