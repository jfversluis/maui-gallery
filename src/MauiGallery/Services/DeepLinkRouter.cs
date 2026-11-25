using MauiGallery.Core.Navigation;
using MauiGallery.Features.Catalog;
using Microsoft.Extensions.Logging;

namespace MauiGallery.Services;

/// <summary>
/// The single entry point for links that open the app (<c>mauigallery://item/{id}</c>), from every platform callback.
/// Links that arrive before a window is ready (a cold start) are held until the first window's Shell has loaded.
/// </summary>
public sealed partial class DeepLinkRouter(
    GalleryCatalog catalog,
    ActiveWindowTracker windows,
    INavigationService navigation,
    ILogger<DeepLinkRouter> logger)
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

    /// <summary>Opens the pending link, if any, once a Shell is ready to navigate.</summary>
    public async Task FlushAsync()
    {
        if (_pendingItemId is not { } itemId || windows.ActiveShell is not { IsLoaded: true })
        {
            return;
        }

        _pendingItemId = null;
        await navigation.GoToAsync($"{Routes.AllControls}/{itemId}");
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Ignored link '{Uri}': not a gallery item")]
    private static partial void LogIgnored(ILogger logger, Uri? uri);
}
