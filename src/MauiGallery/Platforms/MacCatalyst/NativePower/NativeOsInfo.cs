using Foundation;

namespace MauiGallery.Features.NativePower;

public sealed partial class NativeOsInfo
{
    // On Mac Catalyst, UIDevice reports an iPadOS version; NSProcessInfo reports the real macOS one.
    private static partial string ReadBuild() => $"macOS {NSProcessInfo.ProcessInfo.OperatingSystemVersionString}";

    private static partial string? ReadThermalState() => NSProcessInfo.ProcessInfo.ThermalState.ToString();
}
