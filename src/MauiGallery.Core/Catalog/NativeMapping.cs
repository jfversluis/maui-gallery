namespace MauiGallery.Core.Catalog;

/// <summary>Describes the native control a .NET MAUI control maps to on one platform.</summary>
/// <param name="Platform">The platform this mapping applies to.</param>
/// <param name="NativeType">The native type name, for example <c>UIButton</c>.</param>
public sealed record NativeMapping(GalleryPlatforms Platform, string NativeType);
