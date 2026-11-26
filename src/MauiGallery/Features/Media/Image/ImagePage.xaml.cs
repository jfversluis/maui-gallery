using System.Diagnostics;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Media;

public sealed partial class ImagePage : SamplePage
{
    // The dotnet-bot illustrations in github.com/dotnet/brand are licensed CC0 1.0.
    private const string RemoteImageRoot = "https://raw.githubusercontent.com/dotnet/brand/main/spot-illustrations/";
    private const string EmbeddedImageName = "MauiGallery.Images.night_sky.png";

    private readonly Stopwatch _loadTimer = new();

    public ImagePage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        FileExample.NativeTarget = FileImage;
        UriExample.NativeTarget = UriImage;

        UriImage.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Image.IsLoading))
            {
                UriExample.Output = UriImage.IsLoading
                    ? "IsLoading: True"
                    : $"IsLoading: False, after {_loadTimer.ElapsedMilliseconds} ms";
            }
        };
        OnUriOptionsChanged(this, EventArgs.Empty);

        StreamImage.Source = ImageSource.FromStream(OpenEmbeddedImage);
    }

    private void OnUriOptionsChanged(object? sender, EventArgs e)
    {
        var uri = new Uri(RemoteImageRoot + (UriName.SelectedItem as string ?? "bot_jetpack.png"));
        var (validity, xaml, code) = UriValidity.SelectedIndex switch
        {
            0 => (TimeSpan.FromHours(1), "01:00:00", "TimeSpan.FromHours(1)"),
            2 => (TimeSpan.FromDays(7), "7.00:00:00", "TimeSpan.FromDays(7)"),
            _ => (TimeSpan.FromDays(1), null, null), // The default
        };

        _loadTimer.Restart();
        UriImage.Source = new UriImageSource
        {
            Uri = uri,
            CachingEnabled = UriCaching.IsToggled,
            CacheValidity = validity,
        };

        UriText.Value = uri.ToString();
        UriValidityText.Value = UriCaching.IsToggled ? xaml : null;
        UriValidityCode.Value = UriCaching.IsToggled ? code : null;
    }

    private Stream OpenEmbeddedImage()
    {
        var stream = typeof(ImagePage).Assembly.GetManifestResourceStream(EmbeddedImageName)
            ?? throw new FileNotFoundException("Embedded image not found.", EmbeddedImageName);
        var size = stream.Length / 1024.0;
        Dispatcher.Dispatch(() => StreamExample.Output = $"Opened {EmbeddedImageName}, {size:N1} KB");
        return stream;
    }
}
