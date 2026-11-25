using MauiGallery.Services;
using Microsoft.Maui.LifecycleEvents;

namespace MauiGallery;

/// <summary>Forwards links that launch or resume the app, on each platform, to <see cref="DeepLinkRouter"/>.</summary>
internal static class LinkLifecycle
{
    public static MauiAppBuilder ConfigureDeepLinks(this MauiAppBuilder builder) => builder.ConfigureLifecycleEvents(events =>
    {
#if IOS || MACCATALYST
        events.AddiOS(ios => ios
            // Cold start: the scene is created with the URL.
            .SceneWillConnect((_, _, options) =>
            {
                foreach (var context in options.UrlContexts?.ToArray() ?? [])
                {
                    Route(context.Url.AbsoluteString);
                }
            })
            // Warm start: the app is already running.
            .SceneOpenUrl((_, contexts) =>
            {
                foreach (var context in contexts.ToArray())
                {
                    Route(context.Url.AbsoluteString);
                }

                return true;
            }));
#elif ANDROID
        events.AddAndroid(android => android
            .OnCreate((activity, _) => Route(activity.Intent?.DataString))
            .OnNewIntent((_, intent) => Route(intent?.DataString)));
#endif
    });

    private static void Route(string? link)
    {
        if (Uri.TryCreate(link, UriKind.Absolute, out var uri))
        {
            // Lifecycle callbacks sit outside DI, so resolve the router from the running app.
            IPlatformApplication.Current?.Services.GetRequiredService<DeepLinkRouter>().Handle(uri);
        }
    }
}
