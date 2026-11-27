using MauiGallery.Services;

namespace MauiGallery.Features.Media;

public sealed partial class WebViewPage
{
    // See WebViewTeardown: leaving the page while a web view is still starting would otherwise end the app.
    partial void ConfigurePlatform() => WebViewTeardown.Register(this, UrlWebView, HtmlWebView, ScriptWebView);
}
