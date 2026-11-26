using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class GeocodingPage : SamplePage
{
    private static readonly Dictionary<string, (string Latitude, string Longitude)> Places = new()
    {
        ["Eiffel Tower"] = ("48.8584", "2.2945"),
        ["Golden Gate Bridge"] = ("37.8199", "-122.4783"),
        ["Colosseum"] = ("41.8902", "12.4922"),
        ["Taj Mahal"] = ("27.1751", "78.0421"),
    };

    private readonly GeocodingViewModel _viewModel;

    public GeocodingPage(SamplePageServices services, GeocodingViewModel viewModel)
        : base(services)
    {
        _viewModel = viewModel;
        InitializeComponent();
        ForwardSample.BindingContext = viewModel;
        ReverseSample.BindingContext = viewModel;
        WhereSample.BindingContext = viewModel;
        ForwardExample.NativeTarget = ForwardButton;
        ReverseExample.NativeTarget = ReverseButton;
        WhereExample.NativeTarget = WhereButton;

        viewModel.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(GeocodingViewModel.ForwardResult):
                    ForwardExample.Output = $"GetLocationsAsync: {viewModel.ForwardResult}";
                    break;
                case nameof(GeocodingViewModel.ReverseResult):
                    ReverseExample.Output = $"GetPlacemarksAsync: {viewModel.ReverseResult}";
                    break;
                case nameof(GeocodingViewModel.WhereAmI):
                    WhereExample.Output = viewModel.WhereAmI;
                    break;
            }
        };
    }

    private void OnForwardPresetChanged(object? sender, EventArgs e)
    {
        if (ForwardPreset.SelectedItem is string address)
        {
            _viewModel.Address = address;
        }
    }

    private void OnReversePresetChanged(object? sender, EventArgs e)
    {
        if (ReversePreset.SelectedItem is string place)
        {
            (_viewModel.Latitude, _viewModel.Longitude) = Places[place];
        }
    }
}
