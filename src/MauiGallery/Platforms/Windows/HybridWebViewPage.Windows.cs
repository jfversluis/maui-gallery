using Microsoft.Web.WebView2.Core;

namespace MauiGallery.Features.Media;

/// <summary>
/// .NET MAUI 11.0.0 creates HybridWebView's WebView2 environment without the language WinUI gives every other WebView2
/// in the app. WebView2 controls that share a browser process need matching environments, so a HybridWebView next to
/// a WebView or BlazorWebView failed to initialize and crashed the app (dotnet/maui#39512, fixed in .NET MAUI 11.0.10).
/// Use WinUI's language here too. The handler reads these options as soon as it's created, so subscribe beforehand.
/// </summary>
public sealed partial class HybridWebViewPage
{
    partial void ConfigurePlatform()
    {
        InvokeWebView.WebViewInitializing += OnWebViewInitializing;
        GaugeWebView.WebViewInitializing += OnWebViewInitializing;
        MessagesWebView.WebViewInitializing += OnWebViewInitializing;
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
