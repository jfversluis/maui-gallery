using Microsoft.Web.WebView2.Core;

namespace MauiGallery.Features.Media;

/// <summary>
/// Windows workarounds for HybridWebView in .NET MAUI 11.0.0:
/// <list type="bullet">
/// <item>Its WebView2 environment lacks the language WinUI gives every other WebView2 in the app. WebView2 controls
/// that share a browser process need matching environments, so next to a WebView or BlazorWebView it failed to start
/// and crashed the app (dotnet/maui#39512, fixed in 11.0.10). Use WinUI's language here too, before the handler reads
/// the options as it's created.</item>
/// <item>When a page is popped, .NET MAUI disconnects its handlers. If WebView2 is still starting (several seconds on
/// a cold start), the handler later touches its disconnected view and the app crashes. So disconnect the web views
/// ourselves, once they have started.</item>
/// </list>
/// </summary>
public sealed partial class HybridWebViewPage
{
    private readonly HashSet<HybridWebView> _started = [];
    private bool _closed;

    private HybridWebView[] WebViews => [InvokeWebView, GaugeWebView, MessagesWebView];

    partial void ConfigurePlatform()
    {
        foreach (var webView in WebViews)
        {
            webView.WebViewInitializing += OnWebViewInitializing;
            webView.WebViewInitialized += OnWebViewInitialized;
            HandlerProperties.SetDisconnectPolicy(webView, HandlerDisconnectPolicy.Manual);
        }

        NavigatedFrom += OnNavigatedFrom;
    }

    private static void OnWebViewInitializing(object? sender, WebViewInitializingEventArgs e)
    {
        if (e.PlatformArgs is not { } args)
        {
            return;
        }

        var options = args.EnvironmentOptions ?? new CoreWebView2EnvironmentOptions();
        var languages = global::Windows.Globalization.ApplicationLanguages.Languages;
        if (string.IsNullOrEmpty(options.Language) && languages.Count > 0)
        {
            options.Language = languages[0];
        }

        args.EnvironmentOptions = options;
    }

    private void OnWebViewInitialized(object? sender, WebViewInitializedEventArgs e)
    {
        if (sender is HybridWebView webView && _started.Add(webView) && _closed)
        {
            DisconnectSoon(webView);
        }
    }

    private void OnNavigatedFrom(object? sender, NavigatedFromEventArgs e) =>
        // A popped page has no parent once the navigation completes. A page that's still in a stack (another tab, or
        // a page pushed on top) keeps it, and keeps its web views for when the user comes back.
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(500), () =>
        {
            if (Parent is not null || _closed)
            {
                return;
            }

            _closed = true;
            foreach (var webView in _started)
            {
                DisconnectSoon(webView);
            }
        });

    // Leave time for the handler's own continuation, which loads the page once WebView2 has started.
    private void DisconnectSoon(HybridWebView webView) =>
        Dispatcher.DispatchDelayed(TimeSpan.FromSeconds(1), webView.DisconnectHandlers);
}
