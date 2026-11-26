using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class ScreenshotPage : SamplePage
{
    private readonly IScreenshot _screenshot;

    public ScreenshotPage(SamplePageServices services, IScreenshot screenshot)
        : base(services)
    {
        _screenshot = screenshot;
        InitializeComponent();

        WindowExample.NativeTarget = WindowButton;
        ViewExample.NativeTarget = ViewButton;
        ViewFormat.ItemsSource = Enum.GetValues<ScreenshotFormat>();
        ViewFormat.SelectedItem = ScreenshotFormat.Png;

        if (!screenshot.IsCaptureSupported)
        {
            WindowButton.IsEnabled = false;
            WindowInfo.Text = "Screenshots aren't supported on this device.";
        }
    }

    private async void OnCaptureWindowClicked(object? sender, EventArgs e)
    {
        var result = await _screenshot.CaptureAsync();
        var bytes = await ReadAsync(result, ScreenshotFormat.Png, 100);

        WindowPreview.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
        WindowPreview.IsVisible = true;
        WindowInfo.Text = $"{result.Width} × {result.Height}, {bytes.Length / 1024:N0} KB as PNG";
        WindowExample.Output = "CaptureAsync() captured the window";
    }

    private async void OnCaptureViewClicked(object? sender, EventArgs e)
    {
        var result = await Card.CaptureAsync();
        if (result is null)
        {
            ViewInfo.Text = "The card isn't on screen yet.";
            return;
        }

        var format = (ScreenshotFormat)ViewFormat.SelectedItem;
        var quality = (int)Math.Round(ViewQuality.Value);
        var bytes = await ReadAsync(result, format, quality);

        ViewPreview.Source = ImageSource.FromStream(() => new MemoryStream(bytes));
        ViewPreview.IsVisible = true;
        ViewInfo.Text = $"{result.Width} × {result.Height}, {bytes.Length / 1024.0:N1} KB as {format}";
        ViewExample.Output = $"Card.CaptureAsync(), then OpenReadAsync({format}, {quality})";
    }

    private void OnViewFormatChanged(object? sender, EventArgs e)
    {
        var format = (ScreenshotFormat)ViewFormat.SelectedItem;
        FormatSubstitution.Value = $"ScreenshotFormat.{format}";

        // Quality only applies to JPEG.
        ViewQuality.IsEnabled = format == ScreenshotFormat.Jpeg;
        if (format == ScreenshotFormat.Png)
        {
            ViewQuality.Value = 100;
        }
    }

    private static async Task<byte[]> ReadAsync(IScreenshotResult result, ScreenshotFormat format, int quality)
    {
        await using var stream = await result.OpenReadAsync(format, quality);
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        return memory.ToArray();
    }
}
