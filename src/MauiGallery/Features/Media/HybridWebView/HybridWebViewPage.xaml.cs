using System.Text.Json;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Media;

public sealed partial class HybridWebViewPage : SamplePage
{
    private int _lastLevel = -1;

    public HybridWebViewPage(SamplePageServices services, IDeviceInfo deviceInfo)
        : base(services)
    {
        InitializeComponent();

        // The source generator replaces this call with typed, reflection-free invokers for HybridBridge's methods.
        // It can't see fields that XAML source generation creates for x:Name, so call it through a typed local:
        // through InvokeWebView directly the call isn't intercepted and throws at run time.
        HybridWebView webView = InvokeWebView;
        webView.SetInvokeJavaScriptTarget(
            new HybridBridge(deviceInfo, call => Dispatcher.Dispatch(() => InvokeExample.Output = call)),
            HybridJsonContext.Default);

        GaugeExample.NativeTarget = GaugeWebView;
        MessagesExample.NativeTarget = MessagesWebView;
        ConfigurePlatform();
    }

    /// <summary>Platform workarounds, applied before the web views are created (see Platforms/Windows).</summary>
    partial void ConfigurePlatform();

    private async void OnGaugeLevelChanged(object? sender, ValueChangedEventArgs e)
    {
        var level = (int)Math.Round(e.NewValue);
        if (level == _lastLevel)
        {
            return;
        }

        _lastLevel = level;
        try
        {
            var label = await GaugeWebView.InvokeJavaScriptAsync(
                "setLevel",
                HybridJsonContext.Default.String,
                [level],
                [HybridJsonContext.Default.Int32]);
            GaugeExample.Output = $"setLevel({level}) returned \"{label}\"";
        }
        catch (Exception ex)
        {
            // JavaScript errors, or a page that hasn't loaded yet, surface here as an exception.
            GaugeExample.Output = $"setLevel({level}) failed: {ex.Message}";
        }
    }

    private void OnSendClicked(object? sender, EventArgs e)
    {
        var message = new HybridChatMessage(".NET MAUI", MessageText.Text ?? string.Empty);
        MessagesWebView.SendRawMessage(JsonSerializer.Serialize(message, HybridJsonContext.Default.HybridChatMessage));
        MessagesExample.Output = $"SendRawMessage: \"{message.Text}\"";
    }

    private void OnRawMessageReceived(object? sender, HybridWebViewRawMessageReceivedEventArgs e)
    {
        HybridChatMessage? message;
        try
        {
            message = JsonSerializer.Deserialize(e.Message ?? string.Empty, HybridJsonContext.Default.HybridChatMessage);
        }
        catch (JsonException)
        {
            message = null;
        }

        // Treat everything from the page as untrusted input.
        if (message is null || string.IsNullOrWhiteSpace(message.Text))
        {
            Dispatcher.Dispatch(() => MessagesExample.Output = "RawMessageReceived: ignored a malformed message");
            return;
        }

        Dispatcher.Dispatch(() =>
        {
            ReceivedText.Text = $"{message.Author} says: {message.Text}";
            MessagesExample.Output = $"RawMessageReceived: {e.Message}";
        });
    }
}
