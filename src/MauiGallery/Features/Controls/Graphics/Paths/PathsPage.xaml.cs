using MauiGallery.Features.Samples;
using Microsoft.Maui.Controls.Shapes;

namespace MauiGallery.Features.Controls.Graphics;

public sealed partial class PathsPage : SamplePage
{
    private const double Point1X = 80;
    private const double Point2X = 200;
    private const double HandleRadius = 6;

    private static readonly PathGeometryConverter GeometryConverter = new();

    // 24 by 24 icons in path markup, the format of an SVG path's d attribute.
    private static readonly Dictionary<string, string> Icons = new(StringComparer.Ordinal)
    {
        ["Heart"] = "M 12,21 C 12,21 2,14.5 2,8.5 C 2,5.4 4.4,3 7.4,3 C 9.3,3 11,4 12,5.6 C 13,4 14.7,3 16.6,3 C 19.6,3 22,5.4 22,8.5 C 22,14.5 12,21 12,21 Z",
        ["Star"] = "M 12,2 L 14.9,8.3 L 21.8,9.1 L 16.7,13.8 L 18.1,20.6 L 12,17.2 L 5.9,20.6 L 7.3,13.8 L 2.2,9.1 L 9.1,8.3 Z",
        ["Bolt"] = "M 13,2 L 4,14 L 11,14 L 10,22 L 20,9 L 13,9 Z",
        ["Chat"] = "M 5,3 L 19,3 A 3,3 0 0 1 22,6 L 22,15 A 3,3 0 0 1 19,18 L 10,18 L 5,22 L 5,18 A 3,3 0 0 1 2,15 L 2,6 A 3,3 0 0 1 5,3 Z",
    };

    public PathsPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        OnIconChanged(this, EventArgs.Empty);
        OnCurveChanged(this, EventArgs.Empty);
    }

    private void OnIconChanged(object? sender, EventArgs e)
    {
        var data = Icons[IconData.SelectedItem as string ?? "Heart"];
        IconPath.Data = (Geometry?)GeometryConverter.ConvertFromInvariantString(data);
        IconDataText.Value = data;
    }

    private void OnCurveChanged(object? sender, EventArgs e)
    {
        var start = new Point(20, 170);
        var point1 = new Point(Point1X, Math.Round(CurvePoint1.Value));
        var point2 = new Point(Point2X, Math.Round(CurvePoint2.Value));
        var end = Curve.Point3;

        Curve.Point1 = point1;
        Curve.Point2 = point2;
        CurveGuide.Points = [start, point1, point2, end];
        MoveHandle(Handle1, point1);
        MoveHandle(Handle2, point2);
        CurvePoint1Y.Value = point1.Y;
        CurvePoint2Y.Value = point2.Y;
    }

    private static void MoveHandle(Ellipse handle, Point center)
    {
        handle.TranslationX = center.X - HandleRadius;
        handle.TranslationY = center.Y - HandleRadius;
    }
}
