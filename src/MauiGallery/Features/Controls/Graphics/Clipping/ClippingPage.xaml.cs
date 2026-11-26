using MauiGallery.Features.Samples;
using Microsoft.Maui.Controls.Shapes;

namespace MauiGallery.Features.Controls.Graphics;

public sealed partial class ClippingPage : SamplePage
{
    private static readonly PathGeometryConverter GeometryConverter = new();

    // Outlines in the clipped view's coordinates: it is 200 by 200.
    private static readonly Dictionary<string, string> Outlines = new(StringComparer.Ordinal)
    {
        ["Star"] = "M 100,10 L 125,74 L 193,78 L 140,121 L 158,187 L 100,150 L 42,187 L 60,121 L 7,78 L 75,74 Z",
        ["Heart"] = "M 100,195 C 100,195 0,130 0,70 C 0,39 24,15 54,15 C 73,15 90,25 100,41 C 110,25 127,15 146,15 C 176,15 200,39 200,70 C 200,130 100,195 100,195 Z",
        ["Hexagon"] = "M 100,2 L 185,51 L 185,149 L 100,198 L 15,149 L 15,51 Z",
    };

    public ClippingPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        OnRoundChanged(this, EventArgs.Empty);
        OnPathShapeChanged(this, EventArgs.Empty);
    }

    private void OnRoundChanged(object? sender, EventArgs e)
    {
        var radius = Math.Round(RoundRadius.Value);
        var cornerRadius = RoundTopOnly.IsChecked ? new CornerRadius(radius, radius, 0, 0) : new CornerRadius(radius);
        RoundClip.CornerRadius = cornerRadius;
        RoundRadiusText.Value = RoundTopOnly.IsChecked ? $"{radius},{radius},0,0" : radius;
        RoundRadiusCode.Value = RoundTopOnly.IsChecked ? $"new CornerRadius({radius}, {radius}, 0, 0)" : $"{radius}";
    }

    private void OnPathShapeChanged(object? sender, EventArgs e)
    {
        var figures = Outlines[PathShape.SelectedItem as string ?? "Star"];
        PathClipped.Clip = (Geometry?)GeometryConverter.ConvertFromInvariantString(figures);
        PathFiguresText.Value = figures;
    }
}
