using Android.Content.PM;

namespace MauiGallery.Features.Media;

public static partial class MapAvailability
{
    private const string ApiKeyName = "com.google.android.geo.API_KEY";

    // The key is injected into AndroidManifest.xml at build time from the MapsApiKey property; it's empty when unset.
    static partial void CheckApiKey(ref bool hasApiKey)
    {
        var context = Platform.AppContext;
        var packageManager = context.PackageManager;
        if (packageManager is null || context.PackageName is not { } packageName)
        {
            return;
        }

        var info = OperatingSystem.IsAndroidVersionAtLeast(33)
            ? packageManager.GetApplicationInfo(packageName, PackageManager.ApplicationInfoFlags.Of((long)PackageInfoFlags.MetaData))
            : packageManager.GetApplicationInfo(packageName, PackageInfoFlags.MetaData);
        hasApiKey = !string.IsNullOrWhiteSpace(info.MetaData?.GetString(ApiKeyName));
    }
}
