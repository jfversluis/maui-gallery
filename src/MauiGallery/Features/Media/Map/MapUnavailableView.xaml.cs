using IconFont.Maui.FluentIcons;

namespace MauiGallery.Features.Media;

/// <summary>Takes a map's place on the stage when this build can't render one.</summary>
public sealed partial class MapUnavailableView : ContentView
{
    public MapUnavailableView(MapAvailabilityState state)
    {
        InitializeComponent();
        (IconLabel.Text, TitleLabel.Text, MessageLabel.Text) = state switch
        {
            MapAvailabilityState.NotSupported => (
                FluentIconsRegular.Map24,
                "Maps aren't available on Windows",
                ".NET MAUI Maps renders Apple Maps on iOS and Mac and Google Maps on Android, but has no Windows implementation yet. This page checks DeviceInfo.Platform == DevicePlatform.WinUI and shows this instead."),
            _ => (
                FluentIconsRegular.Key24,
                "Map needs an API key",
                "Google Maps on Android needs an API key in the app manifest, and this build has none. Build the gallery with -p:MapsApiKey=<your key> to see the map."),
        };
        SemanticProperties.SetDescription(this, $"{TitleLabel.Text}. {MessageLabel.Text}");
    }
}
