using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class HapticsAndVibrationPage : SamplePage
{
    private readonly HapticsViewModel _viewModel;

    public HapticsAndVibrationPage(SamplePageServices services, HapticsViewModel viewModel)
        : base(services)
    {
        _viewModel = viewModel;
        InitializeComponent();
        HapticSample.BindingContext = viewModel;
        DialSample.BindingContext = viewModel;
        DialOptions.BindingContext = viewModel;
        VibrationSample.BindingContext = viewModel;
        VibrationOptions.BindingContext = viewModel;
        HapticExample.NativeTarget = HapticClick;
        DialExample.NativeTarget = DialSlider;
        VibrationExample.NativeTarget = VibrateButton;

        viewModel.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(HapticsViewModel.LastPerformed):
                    HapticExample.Output = viewModel.LastPerformed;
                    break;
                case nameof(HapticsViewModel.Step):
                    DialExample.Output = viewModel.UseHaptics ? $"Step {viewModel.Step}: performed Click" : $"Step {viewModel.Step}";
                    break;
                case nameof(HapticsViewModel.VibrationStatus):
                    VibrationExample.Output = viewModel.VibrationStatus;
                    break;
            }
        };
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _viewModel.Stop();
    }
}
