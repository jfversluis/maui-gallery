using IconFont.Maui.FluentIcons;

namespace MauiGallery.Features.Media;

/// <summary>Takes a map's place on the stage when this build has no key for the platform's map service.</summary>
public sealed partial class MapUnavailableView : ContentView
{
    public MapUnavailableView()
    {
        InitializeComponent();
        IconLabel.Text = FluentIconsRegular.Key24;
        (TitleLabel.Text, MessageLabel.Text) = DeviceInfo.Platform == DevicePlatform.WinUI
            ? ("Map needs an Azure Maps key",
                "The Windows map renders Azure Maps, which needs a key, and this build has none. Apps register it with UseMapServiceToken. Build the gallery with -p:AzureMapsKey=<your key> to see the map.")
            : ("Map needs an API key",
                "Google Maps on Android needs an API key in the app manifest, and this build has none. Build the gallery with -p:MapsApiKey=<your key> to see the map.");
        SemanticProperties.SetDescription(this, $"{TitleLabel.Text}. {MessageLabel.Text}");
    }
}
