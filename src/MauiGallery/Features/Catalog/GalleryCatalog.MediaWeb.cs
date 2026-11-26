using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Media;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] MediaWebItems() =>
    [
        new()
        {
            Id = "image",
            Title = "Image",
            Description = "Shows pictures from the app package, a font, the web or a stream, sized with Aspect and animated when they are GIFs.",
            CategoryId = CategoryIds.Media,
            IconGlyph = FluentIconsRegular.Image24,
            PageType = typeof(ImagePage),
            Keywords = ["picture", "photo", "ImageSource", "FileImageSource", "FontImageSource", "UriImageSource", "StreamImageSource", "FromStream", "Aspect", "AspectFit", "AspectFill", "MauiImage", "svg", "png", "gif", "IsAnimationPlaying", "IsLoading", "caching", "CacheValidity", "embedded resource", "tint", "TintColor", "icon"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIImageView"),
                new(GalleryPlatforms.Android, "ImageView (loaded with Glide)"),
                new(GalleryPlatforms.MacCatalyst, "UIImageView"),
                new(GalleryPlatforms.Windows, "Image"),
            ],
            RelatedIds = ["image-button", "fonts", "clipping", "media-picker", "graphics-view", "border"],
            DocsUrl = new(DocsRoot + "user-interface/controls/image"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.image"),
            SourceUrl = new(SourceRoot + "Media/Image"),
        },
        new()
        {
            Id = "web-view",
            Title = "WebView",
            Description = "Shows web pages and your own HTML inside the app, with navigation events, history and JavaScript.",
            CategoryId = CategoryIds.Media,
            IconGlyph = FluentIconsRegular.GlobeDesktop24,
            PageType = typeof(WebViewPage),
            Keywords = ["web", "browser", "html", "url", "UrlWebViewSource", "HtmlWebViewSource", "Navigating", "Navigated", "GoBack", "GoForward", "Reload", "CanGoBack", "EvaluateJavaScriptAsync", "JavaScript", "WKWebView", "WebView2", "prefers-color-scheme", "dark mode"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "WKWebView"),
                new(GalleryPlatforms.Android, "android.webkit.WebView"),
                new(GalleryPlatforms.MacCatalyst, "WKWebView"),
                new(GalleryPlatforms.Windows, "WebView2"),
            ],
            RelatedIds = ["hybrid-web-view", "blazor-web-view", "launcher-and-browser", "image"],
            DocsUrl = new(DocsRoot + "user-interface/controls/webview"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.webview"),
            SourceUrl = new(SourceRoot + "Media/WebView"),
        },
        new()
        {
            Id = "hybrid-web-view",
            Title = "HybridWebView",
            Description = "Hosts your own HTML and JavaScript, and lets the page and .NET call each other's methods and exchange messages.",
            CategoryId = CategoryIds.Media,
            IconGlyph = FluentIconsRegular.WindowDevTools24,
            PageType = typeof(HybridWebViewPage),
            Keywords = ["web", "html", "javascript", "js", "interop", "hybrid", "React", "Vue", "Angular", "InvokeDotNet", "InvokeJavaScriptAsync", "SetInvokeJavaScriptTarget", "SendRawMessage", "RawMessageReceived", "HybridRoot", "DefaultFile", "JsonSerializerContext", "source generation", "trimming", "NativeAOT"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "WKWebView"),
                new(GalleryPlatforms.Android, "android.webkit.WebView"),
                new(GalleryPlatforms.MacCatalyst, "WKWebView"),
                new(GalleryPlatforms.Windows, "WebView2"),
            ],
            RelatedIds = ["web-view", "blazor-web-view", "app-and-device-info"],
            DocsUrl = new(DocsRoot + "user-interface/controls/hybridwebview"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.hybridwebview"),
            SourceUrl = new(SourceRoot + "Media/HybridWebView"),
            IsUpdated = true,
        },
        new()
        {
            Id = "blazor-web-view",
            Title = "BlazorWebView",
            Description = "Renders Razor components in a native app and shares services and state with .NET MAUI through dependency injection.",
            CategoryId = CategoryIds.Media,
            IconGlyph = FluentIconsRegular.CodeBlock24,
            PageType = typeof(BlazorWebViewPage),
            Keywords = ["Blazor", "Blazor Hybrid", "Razor", "component", "web", "html", "css", "RootComponent", "HostPage", "wwwroot", "AddMauiBlazorWebView", "@inject", "dependency injection", "StateHasChanged", "InvokeAsync", "Microsoft.AspNetCore.Components.WebView.Maui"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "WKWebView"),
                new(GalleryPlatforms.Android, "android.webkit.WebView"),
                new(GalleryPlatforms.MacCatalyst, "WKWebView"),
                new(GalleryPlatforms.Windows, "WebView2"),
            ],
            RelatedIds = ["hybrid-web-view", "web-view", "mvvm-commands", "bindable-layout"],
            DocsUrl = new(DocsRoot + "user-interface/controls/blazorwebview"),
            ApiUrl = new(ApiRoot + "microsoft.aspnetcore.components.webview.maui.blazorwebview"),
            SourceUrl = new(SourceRoot + "Media/BlazorWebView"),
        },
    ];
}
