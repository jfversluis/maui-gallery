using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class FlashlightPage : SamplePage
{
    private readonly FlashlightViewModel _viewModel;

    public FlashlightPage(SamplePageServices services, FlashlightViewModel viewModel)
        : base(services)
    {
        _viewModel = viewModel;
        InitializeComponent();
        ToggleSample.BindingContext = viewModel;
        SignalSample.BindingContext = viewModel;
        SignalOptions.BindingContext = viewModel;
        ToggleExample.NativeTarget = ToggleButton;
        SignalExample.NativeTarget = SignalButton;

        Appearing += OnPageAppearing;
        Disappearing += OnPageDisappearing;
        viewModel.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(FlashlightViewModel.IsSupported):
                    ToggleExample.Output = $"IsSupportedAsync: {viewModel.IsSupported}";
                    break;
                case nameof(FlashlightViewModel.IsOn) when !viewModel.SignalCommand.IsRunning:
                    ToggleExample.Output = viewModel.IsOn ? "TurnOnAsync" : "TurnOffAsync";
                    break;
                case nameof(FlashlightViewModel.SignalStatus):
                    SignalExample.Output = viewModel.SignalStatus;
                    break;
            }
        };
    }

    private async void OnPageAppearing(object? sender, EventArgs e) => await _viewModel.CheckSupportAsync();

    // Never leave the light on: a lit torch drains the battery and heats the phone.
    private async void OnPageDisappearing(object? sender, EventArgs e) => await _viewModel.TurnOffAsync();
}
