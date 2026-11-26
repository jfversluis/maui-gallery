namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the "Device information" sample.</summary>
public sealed class DeviceInfoViewModel(IDeviceInfo deviceInfo)
{
    public string Model => deviceInfo.Model;

    public string Manufacturer => deviceInfo.Manufacturer;

    public string Name => deviceInfo.Name;

    public DevicePlatform Platform => deviceInfo.Platform;

    public DeviceIdiom Idiom => deviceInfo.Idiom;

    public string Version => deviceInfo.VersionString;

    public DeviceType DeviceType => deviceInfo.DeviceType;
}
