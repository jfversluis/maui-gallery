using System.Net;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Media;

public sealed partial class WebViewPage : SamplePage
{
    // Shared by the HTML samples: follows the app's light or dark theme through prefers-color-scheme.
    private const string Styles = """
        :root { color-scheme: light dark; font-family: system-ui, -apple-system, "Segoe UI", sans-serif; }
        body { margin: 16px; background: #FFFFFF; color: #1F1F1F; }
        .accent { color: #512BD4; }
        @media (prefers-color-scheme: dark) {
          body { background: #1C1B22; color: #F2F1F7; }
          .accent { color: #B3A3FF; }
        }
        """;

    private readonly ILauncher _launcher;

    public WebViewPage(SamplePageServices services, ILauncher launcher)
        : base(services)
    {
        _launcher = launcher;
        InitializeComponent();

        UrlExample.NativeTarget = UrlWebView;
        HtmlExample.NativeTarget = HtmlWebView;
        ScriptExample.NativeTarget = ScriptWebView;

        OnUrlAddressChanged(this, EventArgs.Empty);
        OnUrlExternalToggled(this, new ToggledEventArgs(UrlExternal.IsToggled));
        OnHtmlChanged(this, new TextChangedEventArgs(null, HtmlHeading.Text));
        ScriptWebView.Source = new HtmlWebViewSource
        {
            Html = $$"""
                <!DOCTYPE html>
                <html>
                <head>
                  <meta name="viewport" content="width=device-width, initial-scale=1">
                  <title>Counter</title>
                  <style>{{Styles}} #count { font-size: 32px; }</style>
                </head>
                <body>
                  <p>Count: <b id="count" class="accent">0</b></p>
                  <script>
                    let count = 0;
                    function addOne() {
                      document.getElementById('count').textContent = ++count;
                      return count;
                    }
                  </script>
                </body>
                </html>
                """,
        };
    }

    private string StartAddress => UrlAddress.SelectedItem as string ?? "web/index.html";

    // Bundled pages have no host, so every web link counts as another site.
    private string StartHost => Uri.TryCreate(StartAddress, UriKind.Absolute, out var uri) ? uri.Host : string.Empty;

    private void OnUrlAddressChanged(object? sender, EventArgs e)
    {
        UrlWebView.Source = StartAddress;
        UrlHost.Value = StartHost;
    }

    private void OnUrlExternalToggled(object? sender, ToggledEventArgs e) =>
        UrlExternalLines.Value = e.Value ? string.Empty : null;

    private void OnBackClicked(object? sender, EventArgs e) => UrlWebView.GoBack();

    private void OnForwardClicked(object? sender, EventArgs e) => UrlWebView.GoForward();

    private void OnReloadClicked(object? sender, EventArgs e) => UrlWebView.Reload();

    private async void OnUrlNavigating(object? sender, WebNavigatingEventArgs e)
    {
        if (UrlExternal.IsToggled
            && Uri.TryCreate(e.Url, UriKind.Absolute, out var target)
            && target.Scheme.StartsWith("http", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(target.Host, StartHost, StringComparison.OrdinalIgnoreCase))
        {
            e.Cancel = true;
            UrlExample.Output = $"Navigating: {e.Url}, cancelled and opened in the browser";
            await _launcher.OpenAsync(target);
            return;
        }

        UrlLoading.IsRunning = true;
        UrlStatus.Text = $"Loading {DisplayName(e.Url)}…";
        UrlExample.Output = $"Navigating: {ShortUrl(e.Url)}";
    }

    private void OnUrlNavigated(object? sender, WebNavigatedEventArgs e)
    {
        UrlLoading.IsRunning = false;
        UrlStatus.Text = e.Result switch
        {
            WebNavigationResult.Success => DisplayName(e.Url),
            WebNavigationResult.Cancel => string.Empty,
            _ => $"Couldn't load {DisplayName(e.Url)} ({e.Result}). Check the connection, then Reload.",
        };
        UrlExample.Output = $"Navigated: {ShortUrl(e.Url)} ({e.Result})";
    }

    // Bundled pages load from a long file:// path inside the app package; the file name is enough.
    private static string ShortUrl(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) && IsAppFile(uri) ? $"…/{Path.GetFileName(uri.LocalPath)}" : url;

    private static string DisplayName(string url) =>
        Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme.StartsWith("http", StringComparison.OrdinalIgnoreCase) && !IsAppFile(uri)
            ? uri.Host
            : Path.GetFileName(uri?.LocalPath ?? url);

    // Bundled pages load from a file URL, except on Windows, where .NET MAUI serves them from https://appdir/.
    private static bool IsAppFile(Uri uri) => uri.IsFile || uri.Host == "appdir";

    private void OnHtmlChanged(object? sender, TextChangedEventArgs e)
    {
        var heading = HtmlHeading.Text ?? string.Empty;
        HtmlHeadingText.Value = heading;
        HtmlWebView.Source = new HtmlWebViewSource
        {
            Html = $$"""
                <!DOCTYPE html>
                <html>
                <head>
                  <meta name="viewport" content="width=device-width, initial-scale=1">
                  <style>{{Styles}}</style>
                </head>
                <body>
                  <h2 class="accent">{{WebUtility.HtmlEncode(heading)}}</h2>
                  <ul>
                    <li>Images that load from a stream</li>
                    <li>HTML that follows the app theme</li>
                  </ul>
                </body>
                </html>
                """,
        };
    }

    private async void OnRunScriptClicked(object? sender, EventArgs e)
    {
        var script = ScriptPicker.SelectedItem as string ?? "addOne()";
        var result = await ScriptWebView.EvaluateJavaScriptAsync(script);
        ScriptResult.Text = $"Result: {result}";
        ScriptExample.Output = $"EvaluateJavaScriptAsync(\"{script}\") returned \"{result}\"";
    }
}
