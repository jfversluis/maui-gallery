using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MauiGallery.Features.PlatformApis;

/// <summary>Registers the device services and view models that the permission, location, sensor and media samples inject.</summary>
public static class PlatformApisDeviceSamples
{
    public static MauiAppBuilder UsePlatformApisDeviceSamples(this MauiAppBuilder builder)
    {
        // TryAdd, because the app and other samples register some of these too.
        var services = builder.Services;
        services.TryAddSingleton(AppInfo.Current);
        services.TryAddSingleton(Connectivity.Current);
        services.TryAddSingleton(Geolocation.Default);
        services.TryAddSingleton(Geocoding.Default);
        services.TryAddSingleton(Accelerometer.Default);
        services.TryAddSingleton(Gyroscope.Default);
        services.TryAddSingleton(Magnetometer.Default);
        services.TryAddSingleton(Compass.Default);
        services.TryAddSingleton(Barometer.Default);
        services.TryAddSingleton(OrientationSensor.Default);
        services.TryAddSingleton(HapticFeedback.Default);
        services.TryAddSingleton(Vibration.Default);
        services.TryAddSingleton(Flashlight.Default);
        services.TryAddSingleton(MediaPicker.Default);
        services.TryAddSingleton(FileSystem.Current);

        services
            .AddTransient<CameraAccessViewModel>()
            .AddTransient<GeolocationViewModel>()
            .AddTransient<GeocodingViewModel>()
            .AddTransient<AccelerometerViewModel>()
            .AddTransient<GyroscopeViewModel>()
            .AddTransient<MagnetometerViewModel>()
            .AddTransient<CompassViewModel>()
            .AddTransient<BarometerViewModel>()
            .AddTransient<OrientationViewModel>()
            .AddTransient<HapticsViewModel>()
            .AddTransient<FlashlightViewModel>()
            .AddTransient<MediaPickerViewModel>();

        return builder;
    }
}
