using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.StatusAndInfo;

public sealed partial class ProgressBarPage : SamplePage
{
    private static readonly Dictionary<string, Easing> Easings = new()
    {
        [nameof(Easing.Linear)] = Easing.Linear,
        [nameof(Easing.CubicInOut)] = Easing.CubicInOut,
        [nameof(Easing.SinOut)] = Easing.SinOut,
        [nameof(Easing.BounceOut)] = Easing.BounceOut,
        [nameof(Easing.SpringOut)] = Easing.SpringOut,
    };

    public ProgressBarPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        BasicExample.NativeTarget = BasicProgressBar;
        AnimateExample.NativeTarget = AnimatedBar;
        DownloadSample.BindingContext = new ProgressBarDownloadViewModel();
        DownloadExample.NativeTarget = DownloadBar;
        AnimateEasing.SelectedIndex = 1;
        OnBasicProgressChanged(this, EventArgs.Empty);
    }

    private string SelectedEasing => (string?)AnimateEasing.SelectedItem ?? nameof(Easing.Linear);

    private uint SelectedLength => (uint)Math.Round(AnimateDuration.Value / 50) * 50;

    private void OnBasicProgressChanged(object? sender, EventArgs e) =>
        ProgressSubstitution.Value = Math.Round(BasicProgress.Value, 2);

    private void OnAnimateOptionsChanged(object? sender, EventArgs e)
    {
        EasingSubstitution.Value = $"Easing.{SelectedEasing}";
        LengthSubstitution.Value = SelectedLength;
    }

    private async void OnAnimateClicked(object? sender, EventArgs e)
    {
        AnimatedBar.Progress = 0;
        AnimateExample.Output = "Animating…";

        // ProgressTo returns true when the animation was cancelled, for example by starting another one.
        var cancelled = await AnimatedBar.ProgressTo(1, SelectedLength, Easings[SelectedEasing]);
        if (!cancelled)
        {
            AnimateExample.Output = "ProgressTo finished";
        }
    }
}
