using Microsoft.Maui.Layouts;

namespace MauiGallery.DesignSystem.Layouts;

/// <summary>
/// Lays children out in equal-width columns. The column count follows the available width, so tiles reflow
/// from one column on a phone to several on a desktop without any breakpoint code in the page.
/// </summary>
public sealed class AdaptiveGrid : Layout
{
    public static readonly BindableProperty MinItemWidthProperty =
        BindableProperty.Create(nameof(MinItemWidth), typeof(double), typeof(AdaptiveGrid), 260d, propertyChanged: Invalidate);

    public static readonly BindableProperty MaxColumnsProperty =
        BindableProperty.Create(nameof(MaxColumns), typeof(int), typeof(AdaptiveGrid), 4, propertyChanged: Invalidate);

    public static readonly BindableProperty ColumnSpacingProperty =
        BindableProperty.Create(nameof(ColumnSpacing), typeof(double), typeof(AdaptiveGrid), 16d, propertyChanged: Invalidate);

    public static readonly BindableProperty RowSpacingProperty =
        BindableProperty.Create(nameof(RowSpacing), typeof(double), typeof(AdaptiveGrid), 16d, propertyChanged: Invalidate);

    public double MinItemWidth
    {
        get => (double)GetValue(MinItemWidthProperty);
        set => SetValue(MinItemWidthProperty, value);
    }

    public int MaxColumns
    {
        get => (int)GetValue(MaxColumnsProperty);
        set => SetValue(MaxColumnsProperty, value);
    }

    public double ColumnSpacing
    {
        get => (double)GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
    }

    public double RowSpacing
    {
        get => (double)GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    internal int ColumnsFor(double width, int itemCount)
    {
        if (double.IsInfinity(width) || width <= 0)
        {
            return 1;
        }

        var columns = Math.Clamp((int)((width + ColumnSpacing) / (MinItemWidth + ColumnSpacing)), 1, Math.Max(1, MaxColumns));
        if (itemCount <= columns)
        {
            return columns;
        }

        // Balance the rows: four items in three columns become two rows of two rather than three and one.
        var rows = (int)Math.Ceiling(itemCount / (double)columns);
        return (int)Math.Ceiling(itemCount / (double)rows);
    }

    protected override ILayoutManager CreateLayoutManager() => new AdaptiveGridLayoutManager(this);

    private static void Invalidate(BindableObject bindable, object oldValue, object newValue) =>
        ((AdaptiveGrid)bindable).InvalidateMeasure();

    private sealed class AdaptiveGridLayoutManager(AdaptiveGrid grid) : LayoutManager(grid)
    {
        private double _rowHeightTotal;

        public override Size Measure(double widthConstraint, double heightConstraint)
        {
            var padding = grid.Padding;
            var available = widthConstraint - padding.HorizontalThickness;
            var visible = grid.Where(c => c.Visibility != Visibility.Collapsed).ToList();
            var columns = grid.ColumnsFor(available, visible.Count);
            var itemWidth = ItemWidth(available, columns);

            _rowHeightTotal = 0;
            for (var start = 0; start < visible.Count; start += columns)
            {
                var rowHeight = 0d;
                foreach (var child in visible.Skip(start).Take(columns))
                {
                    rowHeight = Math.Max(rowHeight, child.Measure(itemWidth, double.PositiveInfinity).Height);
                }

                _rowHeightTotal += rowHeight;
            }

            var rows = (int)Math.Ceiling(visible.Count / (double)columns);
            var height = _rowHeightTotal + (Math.Max(0, rows - 1) * grid.RowSpacing) + padding.VerticalThickness;
            var width = double.IsInfinity(widthConstraint) ? (itemWidth * columns) + padding.HorizontalThickness : widthConstraint;
            return new Size(width, height);
        }

        public override Size ArrangeChildren(Rect bounds)
        {
            var padding = grid.Padding;
            var available = bounds.Width - padding.HorizontalThickness;
            var visible = grid.Where(c => c.Visibility != Visibility.Collapsed).ToList();
            var columns = grid.ColumnsFor(available, visible.Count);
            var itemWidth = ItemWidth(available, columns);

            var y = bounds.Top + padding.Top;
            for (var start = 0; start < visible.Count; start += columns)
            {
                var row = visible.Skip(start).Take(columns).ToList();
                var rowHeight = row.Max(c => c.DesiredSize.Height);
                for (var i = 0; i < row.Count; i++)
                {
                    var x = bounds.Left + padding.Left + (i * (itemWidth + grid.ColumnSpacing));
                    row[i].Arrange(new Rect(x, y, itemWidth, rowHeight));
                }

                y += rowHeight + grid.RowSpacing;
            }

            return bounds.Size;
        }

        private double ItemWidth(double available, int columns) =>
            double.IsInfinity(available)
                ? grid.MinItemWidth
                : Math.Max(0, (available - ((columns - 1) * grid.ColumnSpacing)) / columns);
    }
}
