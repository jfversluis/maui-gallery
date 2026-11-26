using System.Globalization;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class GeolocationPage : SamplePage
{
    private static readonly Dictionary<string, Location> Cities = new()
    {
        ["Amsterdam"] = new(52.3676, 4.9041),
        ["London"] = new(51.5072, -0.1276),
        ["New York"] = new(40.7128, -74.0060),
        ["São Paulo"] = new(-23.5558, -46.6396),
        ["Seattle"] = new(47.6062, -122.3321),
        ["Sydney"] = new(-33.8688, 151.2093),
        ["Tokyo"] = new(35.6762, 139.6503),
    };

    private readonly GeolocationViewModel _viewModel;

    public GeolocationPage(SamplePageServices services, GeolocationViewModel viewModel)
        : base(services)
    {
        _viewModel = viewModel;
        InitializeComponent();
        LastKnownSample.BindingContext = viewModel;
        CurrentSample.BindingContext = viewModel;
        CurrentOptions.BindingContext = viewModel;
        ListenSample.BindingContext = viewModel;
        ListenOptions.BindingContext = viewModel;
        LastKnownExample.NativeTarget = LastKnownButton;
        CurrentExample.NativeTarget = CurrentButton;
        ListenExample.NativeTarget = ListenButton;
        DistanceExample.NativeTarget = DistanceValue;

        viewModel.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(GeolocationViewModel.LastKnownCoordinates):
                    LastKnownExample.Output = $"GetLastKnownLocationAsync: {viewModel.LastKnownCoordinates}";
                    break;
                case nameof(GeolocationViewModel.CurrentCoordinates):
                    CurrentExample.Output = $"GetLocationAsync: {viewModel.CurrentCoordinates}";
                    break;
                case nameof(GeolocationViewModel.UpdateCount) when viewModel.UpdateCount > 0:
                    ListenExample.Output = $"LocationChanged #{viewModel.UpdateCount}: {viewModel.ListenCoordinates}";
                    break;
                case nameof(GeolocationViewModel.IsListening):
                    ListenExample.Output = viewModel.IsListening ? "Started listening" : "Stopped listening";
                    break;
            }
        };

        OnDistanceChanged(this, EventArgs.Empty);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Stop();
    }

    private void OnDistanceChanged(object? sender, EventArgs e)
    {
        if (DistanceFrom.SelectedItem is not string fromName || DistanceTo.SelectedItem is not string toName || DistanceUnitsPicker.SelectedItem is not string unitName)
        {
            return;
        }

        var from = Cities[fromName];
        var to = Cities[toName];
        var units = Enum.Parse<DistanceUnits>(unitName);
        var distance = Location.CalculateDistance(from, to, units);

        DistanceValue.Text = $"{distance:N0} {(units == DistanceUnits.Kilometers ? "km" : "mi")}";
        DistanceRoute.Text = $"{fromName} to {toName}, as the crow flies";
        DistanceFromName.Value = fromName;
        DistanceFromLocation.Value = Coordinates(from);
        DistanceToName.Value = toName;
        DistanceToLocation.Value = Coordinates(to);
        DistanceExample.Output = $"CalculateDistance: {distance:N1} {unitName.ToLowerInvariant()}";
    }

    private static string Coordinates(Location location) =>
        string.Create(CultureInfo.InvariantCulture, $"{location.Latitude}, {location.Longitude}");
}
