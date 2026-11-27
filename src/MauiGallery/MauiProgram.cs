using CommunityToolkit.Mvvm.Messaging;
using IconFont.Maui.FluentIcons;
using MauiGallery.DesignSystem.Components;
using MauiGallery.Features.Catalog;
using MauiGallery.Features.Home;
using MauiGallery.Features.Media;
using MauiGallery.Features.NativePower;
using MauiGallery.Features.PlatformApis;
using MauiGallery.Features.Samples;
using MauiGallery.Features.Search;
using MauiGallery.Features.Settings;
using MauiGallery.Features.Chrome;
using MauiGallery.Services;
#if DEBUG
using Microsoft.Extensions.Logging;
using Microsoft.Maui.DevFlow.Agent;
using Microsoft.Maui.DevFlow.Blazor;
#endif

namespace MauiGallery;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        PageLoadTrace.BeginStartup();
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseFluentIcons()
            .UseMauiMaps()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Inter-Regular.ttf", "Inter");
                fonts.AddFont("Inter-Medium.ttf", "InterMedium");
                fonts.AddFont("Inter-SemiBold.ttf", "InterSemiBold");
                fonts.AddFont("Inter-Bold.ttf", "InterBold");
                fonts.AddFont("CascadiaCode-Regular.ttf", "CascadiaCode");
            })
            .ConfigureMauiHandlers(handlers =>
            {
                BorderlessEntry.Register();
#if WINDOWS
                if (Environment.GetEnvironmentVariable("MG_CV1") == "1")
                {
                    handlers.AddHandler<CollectionView, Microsoft.Maui.Controls.Handlers.Items.CollectionViewHandler>();
                }
#endif
            })
            .ConfigureDeepLinks()
            .ConfigureWindowChrome()
            .UseNativePowerSamples()
            .UsePlatformApisCoreSamples()
            .UsePlatformApisDeviceSamples()
            .UseMediaSamples();

        var catalog = new GalleryCatalog();
        builder.Services
            .AddPlatformServices()
            .AddAppServices(catalog)
            .AddShell()
            .AddPages(catalog);

#if DEBUG
        builder.Logging.AddDebug();
        builder.AddMauiDevFlowAgent();
        builder.AddMauiBlazorDevFlowTools();
#endif

        var app = builder.Build();
        PageLoadTrace.Mark("services");
        return app;
    }

    private static IServiceCollection AddPlatformServices(this IServiceCollection services) => services
        .AddSingleton(Preferences.Default)
        .AddSingleton(Browser.Default)
        .AddSingleton(Clipboard.Default)
        .AddSingleton(AppInfo.Current)
        .AddSingleton(DeviceInfo.Current)
        .AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

    private static IServiceCollection AddAppServices(this IServiceCollection services, GalleryCatalog catalog) => services
        .AddSingleton(catalog)
        .AddSingleton<IThemeService, ThemeService>()
        .AddSingleton<IFavoritesService, FavoritesService>()
        .AddSingleton<IRecentsService, RecentsService>()
        .AddSingleton<ActiveWindowTracker>()
        .AddSingleton<INavigationService, ShellNavigationService>()
        .AddSingleton<IWindowService, WindowService>()
        .AddSingleton<IDialogService, DialogService>()
        .AddSingleton<GalleryWindowFactory>()
        .AddSingleton<DeepLinkRouter>()
        .AddSingleton<SamplePageServices>()
        .AddSingleton<ItemActions>();

    private static IServiceCollection AddShell(this IServiceCollection services) => services
        .AddTransient<AppShell>()
        .AddTransient<AppShellViewModel>()
        .AddTransient<SidebarView>()
        .AddTransient<Func<SidebarView>>(services => services.GetRequiredService<SidebarView>)
        .AddTransient<SidebarViewModel>()
        .AddTransient<HomePage>()
        .AddTransient<HomeViewModel>()
        .AddTransient<AllControlsPage>()
        .AddTransient<AllControlsViewModel>()
        .AddTransient<SearchPage>()
        .AddTransient<SearchViewModel>()
        .AddTransient<SettingsPage>()
        .AddTransient<SettingsViewModel>();

    /// <summary>Registers every gallery page with DI and Shell routing, using the catalog id as the route.</summary>
    private static IServiceCollection AddPages(this IServiceCollection services, GalleryCatalog catalog)
    {
        services.AddTransient<CategoryPage>().AddTransient<CategoryViewModel>();
        Routing.RegisterRoute(Routes.Category, typeof(CategoryPage));

        foreach (var item in catalog.Items)
        {
            services.AddTransient(item.PageType);
            Routing.RegisterRoute(item.Id, item.PageType);
        }

        return services;
    }
}
