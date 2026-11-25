using MauiGallery.Core.Navigation;
using MauiGallery.Features.Catalog;
using Microsoft.Extensions.Logging;

namespace MauiGallery.Services;

/// <summary>
/// The single entry point for links that open the app (<c>mauigallery://item/{id}</c>), from every platform callback.
/// Links that arrive before a window is ready (a cold start) are held until <see cref="FlushAsync"/>.
/// </summary>
public sealed partial class DeepLinkRouter(GalleryCatalog catalog, ILogger<DeepLinkRouter> logger)
{
    private string? _pendingItemId;

    public void Handle(Uri? uri)
    {
        var itemId = DeepLink.TryGetItemId(uri);
        if (itemId is null || catalog.Find(itemId) is null)
        {
            LogIgnored(logger, uri);
            return;
        }

        _pendingItemId = itemId;
        MainThread.BeginInvokeOnMainThread(async () => await FlushAsync());
    }

    public async Task FlushAsync()
    {
        if (_pendingItemId is not { } itemId || Shell.Current is not { IsLoaded: true } shell)
        {
            return;
        }

        _pendingItemId = null;
        await shell.GoToAsync($"//main/controls/{itemId}");
        SemanticScreenReader.Announce($"Opened {catalog.Find(itemId)?.Title}");
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Ignored link '{Uri}': not a gallery item")]
    private static partial void LogIgnored(ILogger logger, Uri? uri);
}
