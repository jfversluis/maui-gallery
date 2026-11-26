using Microsoft.Web.WebView2.Core;

namespace MauiGallery;

/// <summary>
/// MAUI 11.0.0 creates HybridWebView's WebView2 environment without the language that WinUI gives every other
/// WebView2. WebView2 controls that share a user data folder need matching environments, so after a WebView has
/// loaded, a HybridWebView fails to initialize and the app crashes (dotnet/maui#39512, fixed in MAUI 11.0.10).
/// Give HybridWebView the same language as WinUI does.
/// </summary>
internal static class HybridWebViewEnvironment
{
    public static void Register() =>
        Microsoft.Maui.Handlers.HybridWebViewHandler.Mapper.AppendToMapping(nameof(HybridWebViewEnvironment), (_, view) =>
        {
            if (view is HybridWebView webView)
            {
                webView.WebViewInitializing -= OnWebViewInitializing;
                webView.WebViewInitializing += OnWebViewInitializing;
            }
        });

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
