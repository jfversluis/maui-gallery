using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the "Open the map app" sample.</summary>
public sealed partial class MapViewModel(IMap map) : ObservableObject
{
    private static readonly Location Rijksmuseum = new(52.3600, 4.8852);

    private static readonly Placemark RijksmuseumAddress = new()
    {
        Thoroughfare = "Museumstraat 1",
        PostalCode = "1071 XX",
        Locality = "Amsterdam",
        CountryName = "Netherlands",
    };

    [ObservableProperty]
    public partial string Name { get; set; } = "Rijksmuseum";

    [ObservableProperty]
    public partial NavigationMode NavigationMode { get; set; }

    [ObservableProperty]
    public partial bool UseAddress { get; set; }

    [ObservableProperty]
    public partial string? Result { get; set; }

    public static IReadOnlyList<NavigationMode> NavigationModes { get; } = Enum.GetValues<NavigationMode>();

    [RelayCommand]
    private async Task OpenMapAsync()
    {
        var options = new MapLaunchOptions { Name = Name, NavigationMode = NavigationMode };
        var opened = UseAddress
            ? await map.TryOpenAsync(RijksmuseumAddress, options)
            : await map.TryOpenAsync(Rijksmuseum, options);

        Result = opened ? "Opened the map app" : "No map app is available";
    }
}
