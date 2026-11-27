using MauiGallery.Services;
using Microsoft.Web.WebView2.Core;

namespace MauiGallery.Features.Media;

/// <summary>
/// Windows workarounds for HybridWebView in .NET MAUI 11.0.0:
/// <list type="bullet">
/// <item>Its WebView2 environment lacks the language WinUI gives every other WebView2 in the app. WebView2 controls
/// that share a browser process need matching environments, so next to a WebView or BlazorWebView it failed to start
/// and crashed the app (dotnet/maui#39512, fixed in 11.0.10). Use WinUI's language here too, before the handler reads
/// the options as it's created.</item>
/// <item>Leaving the page while WebView2 is still starting (several seconds on a cold start) crashes the app, so the
/// web views are disconnected once they have loaded (see <see cref="WebViewTeardown"/>).</item>
/// </list>
/// </summary>
public sealed partial class HybridWebViewPage
{
    partial void ConfigurePlatform()
    {
        HybridWebView[] webViews = [InvokeWebView, GaugeWebView, MessagesWebView];
        foreach (var webView in webViews)
        {
            webView.WebViewInitializing += OnWebViewInitializing;
        }

        WebViewTeardown.Register(this, webViews);
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
}
