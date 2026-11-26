using System.ComponentModel;
using MauiGallery.DesignSystem.Components;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Graphics;

public sealed partial class GraphicsViewPage : SamplePage
{
    private static readonly string[] DayNames = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"];

    private readonly WeeklyChartDrawable _chart = new() { Values = [6200, 8100, 4300, 9400, 7600, 3100, 5800] };
    private readonly GaugeDrawable _gauge = new();
    private readonly SketchDrawable _sketch = new();

    public GraphicsViewPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        ChartView.Drawable = _chart;
        GaugeView.Drawable = _gauge;
        SketchView.Drawable = _sketch;
        SketchView.Loaded += (_, _) => ScrollCapture.Attach(SketchView);

        ChartColor.PropertyChanged += OnSwatchChanged;
        GaugeColor.PropertyChanged += OnSwatchChanged;
        SketchInk.PropertyChanged += OnSwatchChanged;

        OnChartOptionChanged(this, EventArgs.Empty);
        OnGaugeChanged(this, EventArgs.Empty);
        OnSketchSizeChanged(this, EventArgs.Empty);
    }

    private void OnSwatchChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SwatchPicker.SelectedColor))
        {
            return;
        }

        if (sender == ChartColor)
        {
            OnChartOptionChanged(sender, e);
        }
        else if (sender == GaugeColor)
        {
            OnGaugeChanged(sender, e);
        }
        else
        {
            _sketch.InkColor = SketchInk.SelectedColor ?? _sketch.InkColor;
        }
    }

    private void OnChartOptionChanged(object? sender, EventArgs e)
    {
        _chart.IsLine = ChartStyle.SelectedItem as string == "Line";
        _chart.Color = ChartColor.SelectedColor ?? _chart.Color;
        ChartIsLine.Value = _chart.IsLine;
        ChartView.Invalidate();
        DescribeChart();
    }

    private void OnChartRandomize(object? sender, EventArgs e)
    {
        _chart.Values = [.. Enumerable.Range(0, 7).Select(_ => (double)Random.Shared.Next(15, 100) * 100)];
        ChartView.Invalidate();
        var best = DescribeChart();
        ChartExample.Output = $"New data, best day {best}";
    }

    // The drawing is invisible to screen readers, so describe it.
    private string DescribeChart()
    {
        var values = _chart.Values;
        var best = values.Select((value, day) => (value, day)).MaxBy(v => v.value);
        var summary = $"{DayNames[best.day]} ({best.value:N0} steps)";
        SemanticProperties.SetDescription(ChartView, $"Steps per day this week. Most on {summary}.");
        return summary;
    }

    private void OnGaugeChanged(object? sender, EventArgs e)
    {
        _gauge.Value = Math.Round(GaugeValue.Value);
        _gauge.Thickness = (float)Math.Round(GaugeThickness.Value);
        _gauge.Color = GaugeColor.SelectedColor ?? _gauge.Color;
        GaugeView.Invalidate();
        SemanticProperties.SetDescription(GaugeView, $"{_gauge.Value:0} percent");
    }

    private void OnSketchSizeChanged(object? sender, EventArgs e) => _sketch.InkSize = (float)Math.Round(SketchSize.Value);

    private void OnSketchStart(object? sender, TouchEventArgs e)
    {
        ScrollCapture.Begin(SketchView);
        _sketch.StartStroke(e.Touches[0]);
        SketchHint.IsVisible = false;
        SketchView.Invalidate();
    }

    private void OnSketchDrag(object? sender, TouchEventArgs e)
    {
        _sketch.ContinueStroke(e.Touches[0]);
        SketchView.Invalidate();
    }

    private void OnSketchEnd(object? sender, TouchEventArgs e)
    {
        ScrollCapture.End();
        SketchExample.Output = _sketch.StrokeCount == 1 ? "1 stroke" : $"{_sketch.StrokeCount} strokes";
    }

    private void OnSketchCancel(object? sender, EventArgs e) => ScrollCapture.End();

    private void OnSketchClear(object? sender, EventArgs e)
    {
        _sketch.Clear();
        SketchHint.IsVisible = true;
        SketchView.Invalidate();
        SketchExample.Output = "Cleared";
    }
}
