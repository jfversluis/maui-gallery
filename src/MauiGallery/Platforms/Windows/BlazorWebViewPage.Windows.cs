using MauiGallery.Services;

namespace MauiGallery.Features.Media;

public sealed partial class BlazorWebViewPage
{
    // See WebViewTeardown: leaving the page while a web view is still loading would otherwise end the app.
    partial void ConfigurePlatform() => WebViewTeardown.Register(this, CounterWebView, SharedWebView);
}
