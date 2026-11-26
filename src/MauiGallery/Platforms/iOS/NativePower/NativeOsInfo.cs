using Foundation;
using UIKit;

namespace MauiGallery.Features.NativePower;

public sealed partial class NativeOsInfo
{
    private static partial string ReadBuild() =>
        $"{UIDevice.CurrentDevice.SystemName} {NSProcessInfo.ProcessInfo.OperatingSystemVersionString}";

    private static partial string? ReadThermalState() => NSProcessInfo.ProcessInfo.ThermalState.ToString();
}
