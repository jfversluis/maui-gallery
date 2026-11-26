using Android.Content;
using Android.OS;

namespace MauiGallery.Features.NativePower;

public sealed partial class NativeOsInfo
{
    private static partial string ReadBuild() => $"Android {Build.VERSION.Release} (API {(int)Build.VERSION.SdkInt}), build {Build.Display}";

    private static partial string? ReadThermalState()
    {
        if (!OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            return null;
        }

        var power = Android.App.Application.Context.GetSystemService(Context.PowerService) as PowerManager;
        return power?.CurrentThermalStatus switch
        {
            null => null,
            ThermalStatus.None => "None (not throttled)",
            var status => status.ToString(),
        };
    }
}
