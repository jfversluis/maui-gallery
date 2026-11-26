using System.Globalization;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Graphics;

public sealed partial class ShapesPage : SamplePage
{
    public ShapesPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        OnLinesDashChanged(this, EventArgs.Empty);
    }

    private void OnLinesDashChanged(object? sender, EventArgs e)
    {
        var selection = LinesDash.SelectedItem as string ?? "None";
        if (selection == "None")
        {
            SampleLine.StrokeDashArray = [];
            SamplePolyline.StrokeDashArray = [];
            LinesDashText.Value = null;
            LinesDashCode.Value = null;
            return;
        }

        var dashes = selection.Split(',');
        SampleLine.StrokeDashArray = [.. dashes.Select(d => double.Parse(d, CultureInfo.InvariantCulture))];
        SamplePolyline.StrokeDashArray = [.. dashes.Select(d => double.Parse(d, CultureInfo.InvariantCulture))];
        LinesDashText.Value = selection;
        LinesDashCode.Value = $"[{string.Join(", ", dashes)}]";
    }
}
