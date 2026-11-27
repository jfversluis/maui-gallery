using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.UI.Xaml.Controls;

namespace MauiGallery.Features.Media;

/// <summary>
/// Windows workaround for BlazorWebView in .NET MAUI 11.0.0: when a page is closed, .NET MAUI disconnects its handlers,
/// but WebView2 keeps asking the disconnected handler for the page's static files while it is still loading them. The
/// handler's request callback then throws ("VirtualView cannot be null here") and the app ends. So the page disconnects
/// its web views itself, once they have finished loading, and closes them first so no request arrives afterwards.
/// </summary>
public sealed partial class BlazorWebViewPage
{
    private static readonly TimeSpan LoadTimeout = TimeSpan.FromSeconds(10);

    private readonly Dictionary<BlazorWebView, LoadState> _loadStates = [];

    private BlazorWebView[] WebViews => [CounterWebView, SharedWebView];

    partial void ConfigurePlatform()
    {
        if (Environment.GetEnvironmentVariable("MG_NO_BLAZOR_FIX") == "1")
        {
            return;
        }

        foreach (var webView in WebViews)
        {
            var state = new LoadState();
            _loadStates[webView] = state;
            HandlerProperties.SetDisconnectPolicy(webView, HandlerDisconnectPolicy.Manual);
            webView.HandlerChanged += (_, _) => state.Track(webView.Handler?.PlatformView as WebView2);
        }

        NavigatedFrom += OnNavigatedFrom;
    }

    private void OnNavigatedFrom(object? sender, NavigatedFromEventArgs e) =>
        // A popped page has no parent once the navigation completes. A page that's still in a stack (another tab, or
        // a page pushed on top) keeps its web views for when the user comes back.
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(500), () =>
        {
            if (Parent is null)
            {
                NavigatedFrom -= OnNavigatedFrom;
                _ = CloseWebViewsAsync();
            }
        });

    private async Task CloseWebViewsAsync()
    {
        foreach (var webView in WebViews)
        {
            await CloseAsync(webView);
        }
    }

    private async Task CloseAsync(BlazorWebView webView)
    {
        if (webView.Handler?.PlatformView is WebView2 platformView)
        {
            var state = _loadStates[webView];
            try
            {
                await state.Loaded.WaitAsync(LoadTimeout);
            }
            catch (TimeoutException)
            {
                platformView.CoreWebView2?.Stop();
            }

            // Requests still in flight are answered while the handler is connected.
            await Task.Delay(TimeSpan.FromSeconds(1));

            // Closing ends the browser for this view, so no request reaches the handler once it's disconnected.
            platformView.Close();
        }

        webView.DisconnectHandlers();
    }

    /// <summary>Whether a web view has started and finished loading its page.</summary>
    private sealed class LoadState
    {
        private TaskCompletionSource _loaded = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task Loaded => _loaded.Task;

        public void Track(WebView2? platformView)
        {
            if (platformView is null)
            {
                return;
            }

            platformView.CoreWebView2Initialized += (view, _) =>
            {
                if (view.CoreWebView2 is not { } core)
                {
                    return;
                }

                core.NavigationStarting += (_, _) =>
                {
                    if (_loaded.Task.IsCompleted)
                    {
                        _loaded = new(TaskCreationOptions.RunContinuationsAsynchronously);
                    }
                };
                core.NavigationCompleted += (_, _) => _loaded.TrySetResult();
            };
        }
    }
}
