using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MauiGallery.Features.PlatformApis;

/// <summary>Registers the platform API services and view models that the Platform APIs samples inject.</summary>
public static class PlatformApisCoreSamples
{
    public static MauiAppBuilder UsePlatformApisCoreSamples(this MauiAppBuilder builder)
    {
        // Records this launch in the version history at startup, before any page reads it.
        builder.ConfigureEssentials(essentials => essentials.UseVersionTracking());

        // TryAdd, because the app registers some of these for its own use too.
        var services = builder.Services;
        services.TryAddSingleton(AppInfo.Current);
        services.TryAddSingleton(DeviceInfo.Current);
        services.TryAddSingleton(DeviceDisplay.Current);
        services.TryAddSingleton(Battery.Default);
        services.TryAddSingleton(Connectivity.Current);
        services.TryAddSingleton(Clipboard.Default);
        services.TryAddSingleton(Share.Default);
        services.TryAddSingleton(Launcher.Default);
        services.TryAddSingleton(Browser.Default);
        services.TryAddSingleton(Map.Default);
        services.TryAddSingleton(Email.Default);
        services.TryAddSingleton(Sms.Default);
        services.TryAddSingleton(Preferences.Default);
        services.TryAddSingleton(SecureStorage.Default);
        services.TryAddSingleton(VersionTracking.Default);
        services.TryAddSingleton(TextToSpeech.Default);
        services.TryAddSingleton(Screenshot.Default);
        services.TryAddSingleton(FilePicker.Default);
        services.TryAddSingleton(FileSystem.Current);

        services
            .AddTransient<AppInfoViewModel>()
            .AddTransient<DeviceInfoViewModel>()
            .AddTransient<DisplayInfoViewModel>()
            .AddTransient<BatteryViewModel>()
            .AddTransient<ConnectivityViewModel>()
            .AddTransient<ClipboardViewModel>()
            .AddTransient<LauncherViewModel>()
            .AddTransient<MapViewModel>()
            .AddTransient<EmailViewModel>()
            .AddTransient<SmsViewModel>()
            .AddTransient<SettingsStoreViewModel>()
            .AddTransient<SecretsViewModel>()
            .AddTransient<VersionTrackingViewModel>()
            .AddTransient<TextToSpeechViewModel>()
            .AddTransient<FilePickerViewModel>();

        return builder;
    }
}
