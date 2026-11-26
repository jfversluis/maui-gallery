using System.Globalization;
using MauiGallery.Features.Samples;
using Microsoft.Maui.Controls.Shapes;

namespace MauiGallery.Features.Controls.Layouts;

public sealed partial class BorderPage : SamplePage
{
    private static readonly StrokeShapeTypeConverter ShapeConverter = new();

    public BorderPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        OnBasicShapeChanged(this, EventArgs.Empty);
        OnDashArrayChanged(this, EventArgs.Empty);
        OnShadowOffsetChanged(this, EventArgs.Empty);
        ShadowOpacity.ValueChanged += (_, _) => ShadowOpacityText.Value = Math.Round(ShadowOpacity.Value, 2);
        ShadowOpacityText.Value = Math.Round(ShadowOpacity.Value, 2);
    }

    private void OnBasicShapeChanged(object? sender, EventArgs e)
    {
        var shape = BasicShape.SelectedItem as string ?? "Rectangle";

        // XAML converts strings such as "RoundRectangle 12" with this type converter.
        BasicBorder.StrokeShape = (IShape?)ShapeConverter.ConvertFromInvariantString(shape);
        BasicShapeCode.Value = shape switch
        {
            "Rectangle" => "new Rectangle()",
            "Ellipse" => "new Ellipse()",
            _ => $"new RoundRectangle {{ CornerRadius = {ToCornerRadius(shape["RoundRectangle ".Length..])} }}",
        };
    }

    private void OnDashArrayChanged(object? sender, EventArgs e)
    {
        var dashes = (DashArray.SelectedItem as string ?? "4,2").Split(',');
        DashBorder.StrokeDashArray = [.. dashes.Select(d => double.Parse(d, CultureInfo.InvariantCulture))];
        DashArrayCode.Value = $"[{string.Join(", ", dashes)}]";
    }

    private void OnShadowOffsetChanged(object? sender, EventArgs e) =>
        CardShadow.Offset = new Point(0, Math.Round(ShadowOffsetY.Value));

    // "28,4,4,28" becomes "new CornerRadius(28, 4, 4, 28)"; a single value stays as is.
    private static string ToCornerRadius(string radii) =>
        radii.Contains(',', StringComparison.Ordinal) ? $"new CornerRadius({radii.Replace(",", ", ", StringComparison.Ordinal)})" : radii;
}
