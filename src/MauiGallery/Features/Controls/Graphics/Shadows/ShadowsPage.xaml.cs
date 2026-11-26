using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Graphics;

public sealed partial class ShadowsPage : SamplePage
{
    public ShadowsPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        OnCardOffsetChanged(this, EventArgs.Empty);
        OnCardOpacityChanged(this, EventArgs.Empty);
        OnStarElevationChanged(this, EventArgs.Empty);
    }

    private void OnCardOffsetChanged(object? sender, EventArgs e)
    {
        var x = Math.Round(CardOffsetX.Value);
        var y = Math.Round(CardOffsetY.Value);
        CardShadow.Offset = new Point(x, y);
        CardOffsetText.Value = $"{x},{y}";
        CardOffsetCode.Value = $"new Point({x}, {y})";
    }

    private void OnCardOpacityChanged(object? sender, EventArgs e) =>
        CardOpacityText.Value = Math.Round(CardOpacity.Value, 2);

    // Elevation is a design idea, not a MAUI property: a higher object casts a farther, softer shadow.
    private void OnStarElevationChanged(object? sender, EventArgs e)
    {
        var elevation = Math.Round(StarElevation.Value);
        StarShadow.Offset = new Point(0, elevation);
        StarShadow.Radius = (float)elevation;
        StarOffsetY.Value = elevation;
        StarRadius.Value = elevation;
    }
}
