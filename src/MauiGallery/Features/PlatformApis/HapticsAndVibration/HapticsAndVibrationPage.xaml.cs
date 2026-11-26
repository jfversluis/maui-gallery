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
        HapticExample.Output = $"HapticFeedback.IsSupported: {viewModel.IsHapticsSupported}";
        VibrationExample.Output = $"Vibration.IsSupported: {viewModel.IsVibrationSupported}";
        if (!viewModel.IsHapticsSupported)
        {
            HapticExample.Notice = "This device has no haptic engine, so the buttons do nothing here. Try a phone.";
            DialExample.Notice = "This device has no haptic engine: the dial works, without the feel.";
        }

        if (!viewModel.IsVibrationSupported)
        {
            VibrationExample.Notice = "This device can't vibrate, so the buttons do nothing here. Try a phone.";
        }

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
