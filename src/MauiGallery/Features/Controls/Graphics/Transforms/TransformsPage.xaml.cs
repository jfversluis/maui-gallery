using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Graphics;

public sealed partial class TransformsPage : SamplePage
{
    private const double StageWidth = 280;
    private const double StageHeight = 220;
    private const double CardWidth = 120;
    private const double CardHeight = 80;
    private const double DotRadius = 6;

    public TransformsPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        OnBasicScaleChanged(this, EventArgs.Empty);
        OnAnchorChanged(this, EventArgs.Empty);
    }

    private void OnBasicScaleChanged(object? sender, EventArgs e) =>
        BasicScaleText.Value = Math.Round(BasicScale.Value, 2);

    private void OnAnchorChanged(object? sender, EventArgs e)
    {
        var anchorX = Math.Round(AnchorXSlider.Value, 2);
        var anchorY = Math.Round(AnchorYSlider.Value, 2);

        // The anchor doesn't move when the card rotates or scales around it, so the dot can sit on the laid-out card.
        AnchorDot.TranslationX = ((StageWidth - CardWidth) / 2) + (anchorX * CardWidth) - DotRadius;
        AnchorDot.TranslationY = ((StageHeight - CardHeight) / 2) + (anchorY * CardHeight) - DotRadius;

        AnchorXText.Value = anchorX;
        AnchorYText.Value = anchorY;
        AnchorScaleXText.Value = Math.Round(AnchorScaleX.Value, 2);
        AnchorScaleYText.Value = Math.Round(AnchorScaleY.Value, 2);
    }

    private async void OnSpinClicked(object? sender, EventArgs e)
    {
        Restart();
        await AnimatedCard.RotateToAsync(360, 800, Easing.CubicInOut);
        AnimatedCard.Rotation = 0;
        AnimateExample.Output = "Spin finished";
    }

    private async void OnPulseClicked(object? sender, EventArgs e)
    {
        Restart();
        await AnimatedCard.ScaleToAsync(1.25, 150, Easing.CubicOut);
        await AnimatedCard.ScaleToAsync(1, 400, Easing.SpringOut);
        AnimateExample.Output = "Pulse finished";
    }

    private async void OnFlipClicked(object? sender, EventArgs e)
    {
        Restart();
        await AnimatedCard.RotateYToAsync(360, 800, Easing.CubicInOut);
        AnimatedCard.RotationY = 0;
        AnimateExample.Output = "Flip finished";
    }

    private async void OnShakeClicked(object? sender, EventArgs e)
    {
        Restart();
        foreach (var offset in (double[])[-16, 16, -10, 10, -4, 0])
        {
            await AnimatedCard.TranslateToAsync(offset, 0, 60);
        }

        AnimateExample.Output = "Shake finished";
    }

    // A new animation replaces a running one, so it starts from the card's resting state.
    private void Restart()
    {
        AnimatedCard.CancelAnimations();
        AnimatedCard.Rotation = 0;
        AnimatedCard.RotationY = 0;
        AnimatedCard.Scale = 1;
        AnimatedCard.TranslationX = 0;
    }
}
