using System.ComponentModel;
using System.Globalization;
using MauiGallery.Features.Samples;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace MauiGallery.Features.Media;

public sealed partial class MapPage : SamplePage
{
    private static readonly Location Amsterdam = new(52.3676, 4.9041);

    private static readonly Dictionary<string, Location> Cities = new()
    {
        ["Amsterdam"] = Amsterdam,
        ["London"] = new(51.5072, -0.1276),
        ["New York"] = new(40.7128, -74.0060),
        ["São Paulo"] = new(-23.5558, -46.6396),
        ["Seattle"] = new(47.6062, -122.3321),
        ["Sydney"] = new(-33.8688, 151.2093),
        ["Tokyo"] = new(35.6762, 139.6503),
    };

    private readonly List<Pin> _droppedPins = [];

    public MapPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        BasicExample.NativeTarget = BasicMap;
        RegionExample.NativeTarget = RegionMap;
        UserExample.NativeTarget = UserMap;
        PinsExample.NativeTarget = PinsMap;
        ShapesExample.NativeTarget = ShapesMap;

        var availability = MapAvailability.Check();
        if (availability != MapAvailabilityState.Available)
        {
            // Replace each map before it gets a handler: creating one would fail on Windows and show an empty grey
            // map on Android without a key. The code stays, so the page still teaches the API.
            foreach (var (example, stage) in new[] { (BasicExample, BasicStage), (RegionExample, RegionStage), (UserExample, UserStage), (PinsExample, PinsStage), (ShapesExample, ShapesStage) })
            {
                stage.Children.Clear();
                stage.Children.Add(new MapUnavailableView(availability));
                example.Options = null;
            }

            return;
        }

        BasicMap.MoveToRegion(MapSpan.FromCenterAndRadius(Amsterdam, Distance.FromKilometers(3)), animated: false);
        UserMap.MoveToRegion(MapSpan.FromCenterAndRadius(Amsterdam, Distance.FromKilometers(3)), animated: false);
        PinsMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(52.3700, 4.8920), Distance.FromKilometers(1.6)), animated: false);
        ShapesMap.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(52.3670, 4.8800), Distance.FromKilometers(1.6)), animated: false);
        ShapesCircle.Radius = Distance.FromMeters(ShapesRadius.Value);

        RegionMap.PropertyChanged += OnRegionMapPropertyChanged;
        RegionCity.SelectedIndexChanged += OnRegionChanged;
        RegionRadius.ValueChanged += OnRegionChanged;
        ShapesRadius.ValueChanged += OnCircleRadiusChanged;
        MoveToCity(animated: false);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // The blue dot keeps location updates running; stop them when the page goes away.
        UserShowing.IsToggled = false;
    }

    private void OnRegionChanged(object? sender, EventArgs e) => MoveToCity(RegionAnimated.IsToggled);

    private void MoveToCity(bool animated)
    {
        if (RegionCity.SelectedItem is not string city)
        {
            return;
        }

        var center = Cities[city];
        var radius = Math.Round(RegionRadius.Value);
        RegionMap.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(radius)), animated);
        RegionCityName.Value = city;
        RegionCenter.Value = string.Create(CultureInfo.InvariantCulture, $"{center.Latitude}, {center.Longitude}");
    }

    private void OnRegionMapPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Map.VisibleRegion) && RegionMap.VisibleRegion is { } region)
        {
            RegionExample.Output = string.Create(
                CultureInfo.InvariantCulture,
                $"VisibleRegion: {region.Center.Latitude:F4}, {region.Center.Longitude:F4} · radius {region.Radius.Kilometers:N1} km");
        }
    }

    private async void OnShowingUserToggled(object? sender, ToggledEventArgs e)
    {
        UserError.IsVisible = false;
        if (!e.Value)
        {
            UserMap.IsShowingUser = false;
            return;
        }

        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }

        if (status != PermissionStatus.Granted)
        {
            UserShowing.IsToggled = false;
            UserError.Text = "Location access is off. Turn it on in Settings to see yourself on the map.";
            UserError.IsVisible = true;
            SemanticScreenReader.Announce(UserError.Text);
            return;
        }

        UserMap.IsShowingUser = true;
        UserExample.Output = "IsShowingUser: True. Waiting for the first location…";
    }

    private void OnUserLocationChanged(object? sender, UserLocationChangedEventArgs e)
    {
        UserExample.Output = string.Create(CultureInfo.InvariantCulture, $"UserLocationChanged: {e.Location.Latitude:F4}, {e.Location.Longitude:F4}");
        UserMap.MoveToRegion(MapSpan.FromCenterAndRadius(e.Location, Distance.FromKilometers(1)));
    }

    private void OnMarkerClicked(object? sender, PinClickedEventArgs e)
    {
        var pin = (Pin)sender!;
        e.HideInfoWindow = PinsHideInfo.IsToggled;
        PinsExample.Output = $"MarkerClicked: {pin.Label}";
    }

    private void OnInfoWindowClicked(object? sender, PinClickedEventArgs e) =>
        PinsExample.Output = $"InfoWindowClicked: {((Pin)sender!).Label}";

    private void OnMapClicked(object? sender, MapClickedEventArgs e)
    {
        var pin = new Pin
        {
            Label = $"Dropped pin {_droppedPins.Count + 1}",
            Address = string.Create(CultureInfo.InvariantCulture, $"{e.Location.Latitude:F4}, {e.Location.Longitude:F4}"),
            Location = e.Location,
            Type = PinType.SavedPin,
        };
        pin.MarkerClicked += OnMarkerClicked;
        pin.InfoWindowClicked += OnInfoWindowClicked;
        _droppedPins.Add(pin);
        PinsMap.Pins.Add(pin);
        PinsExample.Output = $"MapClicked: {pin.Address}";
    }

    private void OnClearPinsClicked(object? sender, EventArgs e)
    {
        foreach (var pin in _droppedPins)
        {
            PinsMap.Pins.Remove(pin);
        }

        PinsExample.Output = $"Removed {_droppedPins.Count} dropped {(_droppedPins.Count == 1 ? "pin" : "pins")}";
        _droppedPins.Clear();
    }

    private void OnCircleRadiusChanged(object? sender, ValueChangedEventArgs e) =>
        ShapesCircle.Radius = Distance.FromMeters(Math.Round(e.NewValue));

    private void OnShapeClicked(object? sender, EventArgs e) => ShapesExample.Output = sender switch
    {
        Polyline => "PolylineClicked: the walking route",
        Polygon => "PolygonClicked: Vondelpark",
        Circle circle => $"CircleClicked: {circle.Radius.Meters:N0} m around Dam Square",
        _ => null,
    };
}
