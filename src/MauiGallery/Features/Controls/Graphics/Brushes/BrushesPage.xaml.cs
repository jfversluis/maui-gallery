using MauiGallery.DesignSystem.Components;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Graphics;

public sealed partial class BrushesPage : SamplePage
{
    public BrushesPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        SolidColor.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SwatchPicker.SelectedColor))
            {
                OnSolidChanged(this, EventArgs.Empty);
            }
        };
        OnSolidChanged(this, EventArgs.Empty);
        OnLinearDirectionChanged(this, EventArgs.Empty);
        OnRadialCenterChanged(this, EventArgs.Empty);
        OnRadialRadiusChanged(this, EventArgs.Empty);
    }

    private void OnSolidChanged(object? sender, EventArgs e)
    {
        var color = (SolidColor.SelectedColor ?? Colors.Teal).WithAlpha((float)Math.Round(SolidAlpha.Value, 2));
        SolidBox.Background = new SolidColorBrush(color);
        SolidColorText.Value = color;
    }

    private void OnLinearDirectionChanged(object? sender, EventArgs e)
    {
        var end = (LinearDirection.SelectedItem as string) switch
        {
            "Vertical" => new Point(0, 1),
            "Diagonal" => new Point(1, 1),
            _ => new Point(1, 0),
        };
        LinearBrush.EndPoint = end;
        LinearEndPoint.Value = FormattableString.Invariant($"{end.X},{end.Y}");
        LinearEndPointCode.Value = FormattableString.Invariant($"new Point({end.X}, {end.Y})");
    }

    private void OnRadialCenterChanged(object? sender, EventArgs e)
    {
        var center = new Point(Math.Round(RadialCenterX.Value, 2), Math.Round(RadialCenterY.Value, 2));
        RadialBrush.Center = center;
        RadialCenterText.Value = FormattableString.Invariant($"{center.X},{center.Y}");
        RadialCenterCode.Value = FormattableString.Invariant($"new Point({center.X}, {center.Y})");
    }

    private void OnRadialRadiusChanged(object? sender, EventArgs e) =>
        RadialRadiusText.Value = Math.Round(RadialRadius.Value, 2);
}
