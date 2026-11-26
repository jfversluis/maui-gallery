using Microsoft.Maui.Layouts;

namespace MauiGallery.Features.Controls.Layouts;

/// <summary>Places its children evenly around a circle, like the numbers on a clock face.</summary>
public sealed class CircularLayout : Microsoft.Maui.Controls.Layout
{
    public static readonly BindableProperty StartAngleProperty =
        BindableProperty.Create(nameof(StartAngle), typeof(double), typeof(CircularLayout), 0d,
            propertyChanged: (bindable, _, _) => ((CircularLayout)bindable).InvalidateMeasure());

    /// <summary>Where the first child goes, in degrees clockwise from the top.</summary>
    public double StartAngle
    {
        get => (double)GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }

    protected override ILayoutManager CreateLayoutManager() => new CircularLayoutManager(this);
}

/// <summary>Measures and arranges the children of a <see cref="CircularLayout"/>.</summary>
public sealed class CircularLayoutManager(CircularLayout layout) : ILayoutManager
{
    public Size Measure(double widthConstraint, double heightConstraint)
    {
        // Every child gets the size it wants.
        var largest = 0d;
        foreach (var child in layout)
        {
            var size = child.Measure(double.PositiveInfinity, double.PositiveInfinity);
            largest = Math.Max(largest, Math.Max(size.Width, size.Height));
        }

        // The layout is a square that fits the space it's given (or a WidthRequest/HeightRequest).
        var side = Math.Min(widthConstraint, heightConstraint);
        if (double.IsInfinity(side))
        {
            side = largest * 4;
        }

        return new Size(side, side);
    }

    public Size ArrangeChildren(Rect bounds)
    {
        if (layout.Count == 0)
        {
            return bounds.Size;
        }

        // The circle runs through the children's centers, inset so the largest child stays inside the bounds.
        var largest = layout.Max(child => Math.Max(child.DesiredSize.Width, child.DesiredSize.Height));
        var radius = (Math.Min(bounds.Width, bounds.Height) - largest) / 2;
        var step = 360d / layout.Count;

        for (var i = 0; i < layout.Count; i++)
        {
            var child = layout[i];
            var radians = (layout.StartAngle + (i * step) - 90) * Math.PI / 180;
            var size = child.DesiredSize;
            var x = bounds.Center.X + (radius * Math.Cos(radians)) - (size.Width / 2);
            var y = bounds.Center.Y + (radius * Math.Sin(radians)) - (size.Height / 2);
            child.Arrange(new Rect(x, y, size.Width, size.Height));
        }

        return bounds.Size;
    }
}
