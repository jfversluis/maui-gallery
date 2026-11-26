namespace MauiGallery.Features.Media;

/// <summary>Registers the services that the Media and web samples use.</summary>
public static class MediaSamples
{
    public static MauiAppBuilder UseMediaSamples(this MauiAppBuilder builder)
    {
        builder.Services.AddMauiBlazorWebView();
#if DEBUG
        // Lets Safari Web Inspector and Edge DevTools attach to BlazorWebView. Never ship it in Release builds.
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        // Shared by the Razor component and the .NET MAUI controls of the BlazorWebView sample.
        builder.Services.AddSingleton<PackingListViewModel>();
        return builder;
    }
}
