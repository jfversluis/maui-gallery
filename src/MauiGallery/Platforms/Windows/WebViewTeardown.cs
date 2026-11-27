using Microsoft.UI.Xaml.Controls;

namespace MauiGallery.Services;

/// <summary>
/// Windows workaround for .NET MAUI 11.0.0: WebView, BlazorWebView and HybridWebView finish work on their WebView2 after
/// it has started, which takes several seconds on a cold start: WebView syncs cookies once it can load its URL,
/// BlazorWebView serves the page's files, HybridWebView loads its page. That work reads the handler's VirtualView, so if
/// .NET MAUI has disconnected the handler because the page was closed in the meantime, it throws on the UI thread and
/// the app ends ("VirtualView cannot be null here"). A page registers its web views here instead: they are disconnected
/// once they have finished loading.
/// </summary>
public static class WebViewTeardown
{
    private static readonly TimeSpan LoadTimeout = TimeSpan.FromSeconds(15);

    // Time for work that was still in flight when loading completed, such as a Blazor page's last requests.
    private static readonly TimeSpan Settle = TimeSpan.FromSeconds(1);

    public static void Register(Page page, params View[] webViews)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(webViews);

        var states = webViews.Select(webView => (WebView: webView, State: new LoadState())).ToArray();
        foreach (var (webView, state) in states)
        {
            HandlerProperties.SetDisconnectPolicy(webView, HandlerDisconnectPolicy.Manual);
            webView.HandlerChanged += (_, _) => state.Track(webView.Handler?.PlatformView as WebView2);
        }

        page.NavigatedFrom += OnNavigatedFrom;

        void OnNavigatedFrom(object? sender, NavigatedFromEventArgs e) =>
            // A popped page has no parent once the navigation completes. A page that's still in a stack (another tab,
            // or a page pushed on top) keeps its web views for when the user comes back.
            page.Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(500), () =>
            {
                if (page.Parent is not null)
                {
                    return;
                }

                page.NavigatedFrom -= OnNavigatedFrom;
                foreach (var (webView, state) in states)
                {
                    _ = DisconnectWhenLoadedAsync(webView, state);
                }
            });
    }

    private static async Task DisconnectWhenLoadedAsync(View webView, LoadState state)
    {
        try
        {
            await state.Loaded.WaitAsync(LoadTimeout);
        }
        catch (TimeoutException)
        {
            // Never started or never finished: disconnect anyway rather than keep it forever.
        }

        await Task.Delay(Settle);

        // DisconnectHandlers skips views whose policy is Manual.
        HandlerProperties.SetDisconnectPolicy(webView, HandlerDisconnectPolicy.Automatic);
        webView.DisconnectHandlers();
    }

    /// <summary>Completes once a web view has started and finished loading its page.</summary>
    private sealed class LoadState
    {
        private readonly TaskCompletionSource _loaded = new(TaskCreationOptions.RunContinuationsAsynchronously);

        public Task Loaded => _loaded.Task;

        public void Track(WebView2? platformView)
        {
            if (platformView is null)
            {
                return;
            }

            platformView.CoreWebView2Initialized += (view, _) =>
            {
                if (view.CoreWebView2 is { } core)
                {
                    core.NavigationCompleted += (_, _) => _loaded.TrySetResult();
                }
            };
        }
    }
}
