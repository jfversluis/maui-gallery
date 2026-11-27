namespace MauiGallery.Features.NativePower;

/// <summary>Registers the handlers, handler customizations and services that the "Handlers and native views" samples use.</summary>
public static class NativePowerSamples
{
    public static MauiAppBuilder UseNativePowerSamples(this MauiAppBuilder builder)
    {
        builder.ConfigureMauiHandlers(handlers =>
        {
            handlers.AddHandler<NativeCalendar, NativeCalendarHandler>();
            handlers.AddHandler<PlatformViewHost, PlatformViewHostHandler>();
        });

        // Mapper changes are global and permanent, so make them once, at startup.
        PlainEntry.Customize();
        NativeTint.Customize();
        HapticSlider.Customize();

        builder.Services.AddSingleton<INativeOsInfo, NativeOsInfo>();
        return builder;
    }
}
