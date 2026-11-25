using IconFont.Maui.FluentIcons;
using MauiGallery.Services;
#if DEBUG
using Microsoft.Extensions.Logging;
using Microsoft.Maui.DevFlow.Agent;
#endif

namespace MauiGallery;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseFluentIcons()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Inter-Regular.ttf", "Inter");
                fonts.AddFont("Inter-Medium.ttf", "InterMedium");
                fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                fonts.AddFont("Inter-Bold.ttf", "InterBold");
                fonts.AddFont("CascadiaCode-Regular.ttf", "CascadiaCode");
            });

        builder.Services.AddSingleton(Preferences.Default);
        builder.Services.AddSingleton<IThemeService, ThemeService>();

#if DEBUG
        builder.Logging.AddDebug();
        builder.AddMauiDevFlowAgent();
#endif

        return builder.Build();
    }
}
