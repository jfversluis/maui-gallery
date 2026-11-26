using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the Geocoding samples: addresses to coordinates, and back.</summary>
public sealed partial class GeocodingViewModel(IGeocoding geocoding, IGeolocation geolocation, IConnectivity connectivity) : ObservableObject
{
    // Address to coordinates
    [ObservableProperty]
    public partial string Address { get; set; } = "Rijksmuseum, Amsterdam";

    [ObservableProperty]
    public partial string ForwardResult { get; set; } = "Search to see the coordinates";

    [ObservableProperty]
    public partial string? ForwardDetails { get; set; }

    // Coordinates to address
    [ObservableProperty]
    public partial string Latitude { get; set; } = "48.8584";

    [ObservableProperty]
    public partial string Longitude { get; set; } = "2.2945";

    [ObservableProperty]
    public partial string ReverseResult { get; set; } = "Look up to see the address";

    [ObservableProperty]
    public partial string? ReverseDetails { get; set; }

    // Where am I
    [ObservableProperty]
    public partial string WhereAmI { get; set; } = "Find out where you are";

    [ObservableProperty]
    public partial string? WhereAmIDetails { get; set; }

    [RelayCommand]
    private async Task FindCoordinatesAsync()
    {
        ForwardDetails = null;
        if (string.IsNullOrWhiteSpace(Address))
        {
            ForwardResult = "Type an address first";
            return;
        }

        ForwardResult = await RunAsync(async () =>
        {
            var locations = (await geocoding.GetLocationsAsync(Address)).ToList();
            if (locations.Count == 0)
            {
                return "No matches";
            }

            ForwardDetails = locations.Count == 1 ? "1 match" : $"{locations.Count} matches, best first";
            return LocationText.Coordinates(locations[0]);
        });
    }

    [RelayCommand]
    private async Task FindAddressAsync()
    {
        ReverseDetails = null;
        if (!double.TryParse(Latitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var latitude) || latitude is < -90 or > 90
            || !double.TryParse(Longitude, NumberStyles.Float, CultureInfo.InvariantCulture, out var longitude) || longitude is < -180 or > 180)
        {
            ReverseResult = "Use a latitude from -90 to 90 and a longitude from -180 to 180";
            return;
        }

        ReverseResult = await RunAsync(async () =>
        {
            var placemark = (await geocoding.GetPlacemarksAsync(latitude, longitude)).FirstOrDefault();
            ReverseDetails = placemark is null ? null : Describe(placemark);
            return placemark is null ? "No address here" : Headline(placemark);
        });
    }

    [RelayCommand]
    private async Task FindWhereAmIAsync()
    {
        WhereAmIDetails = null;
        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            WhereAmI = "Location access is off. Turn it on in Settings to try this.";
            return;
        }

        WhereAmI = "Locating…";
        WhereAmI = await RunAsync(async () =>
        {
            var location = await geolocation.GetLocationAsync(new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10)));
            if (location is null)
            {
                return "Couldn't get a location";
            }

            var placemark = (await geocoding.GetPlacemarksAsync(location)).FirstOrDefault();
            WhereAmIDetails = LocationText.Coordinates(location);
            return placemark is null ? "Somewhere without an address" : $"You're in {Join(placemark.Locality, placemark.CountryName)}";
        });
    }

    private async Task<string> RunAsync(Func<Task<string>> lookup)
    {
        // Geocoding asks an online service; say so up front rather than waiting for a timeout.
        if (connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            return "You're offline. Geocoding needs the internet.";
        }

        try
        {
            return await lookup();
        }
        catch (FeatureNotSupportedException)
        {
            return "This device has no geocoding service.";
        }
        catch (FeatureNotEnabledException)
        {
            return "Location services are turned off for this device.";
        }
        catch (PermissionException)
        {
            return "Location access is off. Turn it on in Settings to try this.";
        }
        catch (Exception ex)
        {
            // The platform geocoders throw their own exception types when the service fails or is unreachable.
            return $"The lookup failed: {ex.Message}";
        }
    }

    private static string Headline(Placemark placemark) =>
        Join(placemark.FeatureName, placemark.Locality, placemark.CountryName);

    // Every field that has a value; which ones do depends on the place and the platform's geocoder.
    private static string Describe(Placemark placemark) => string.Join(Environment.NewLine,
        new (string Name, string? Value)[]
        {
            (nameof(placemark.SubThoroughfare), placemark.SubThoroughfare),
            (nameof(placemark.Thoroughfare), placemark.Thoroughfare),
            (nameof(placemark.PostalCode), placemark.PostalCode),
            (nameof(placemark.SubLocality), placemark.SubLocality),
            (nameof(placemark.Locality), placemark.Locality),
            (nameof(placemark.AdminArea), placemark.AdminArea),
            (nameof(placemark.CountryCode), placemark.CountryCode),
        }
        .Where(field => !string.IsNullOrWhiteSpace(field.Value))
        .Select(field => $"{field.Name}: {field.Value}"));

    private static string Join(params string?[] parts) =>
        string.Join(", ", parts.Where(p => !string.IsNullOrWhiteSpace(p)).Distinct());
}
