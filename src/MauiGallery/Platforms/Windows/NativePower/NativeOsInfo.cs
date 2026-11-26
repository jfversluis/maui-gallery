using System.Globalization;
using Windows.System.Profile;

namespace MauiGallery.Features.NativePower;

public sealed partial class NativeOsInfo
{
    private static partial string ReadBuild()
    {
        // DeviceFamilyVersion packs the four version numbers into one 64-bit integer.
        var version = ulong.Parse(AnalyticsInfo.VersionInfo.DeviceFamilyVersion, CultureInfo.InvariantCulture);
        return $"Windows {(version >> 48) & 0xFFFF}.{(version >> 32) & 0xFFFF}.{(version >> 16) & 0xFFFF}.{version & 0xFFFF}";
    }

    // Windows apps can't read the device's thermal state.
    private static partial string? ReadThermalState() => null;
}
