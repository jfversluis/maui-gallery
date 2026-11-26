namespace MauiGallery.Features.Controls.Graphics;

/// <summary>Draws a week of values as bars or a line. Change a property, then call Invalidate on the GraphicsView.</summary>
public sealed class WeeklyChartDrawable : IDrawable
{
    private const float LabelHeight = 24;
    private static readonly string[] Days = ["M", "T", "W", "T", "F", "S", "S"];

    // Mid gray reads on light and dark backgrounds alike.
    private static readonly Color AxisColor = Color.FromArgb("#8E8E93");
    private static readonly Color GridColor = Color.FromArgb("#4D8E8E93");

    public IReadOnlyList<double> Values { get; set; } = [];

    public double Maximum { get; set; } = 10_000;

    public Color Color { get; set; } = Color.FromArgb("#512BD4");

    public bool IsLine { get; set; }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var plot = new RectF(dirtyRect.X, dirtyRect.Y + 8, dirtyRect.Width, dirtyRect.Height - LabelHeight - 8);

        canvas.StrokeColor = GridColor;
        canvas.StrokeSize = 1;
        for (var i = 0; i <= 4; i++)
        {
            var y = plot.Bottom - (plot.Height * i / 4);
            canvas.DrawLine(plot.Left, y, plot.Right, y);
        }

        if (Values.Count == 0)
        {
            return;
        }

        var slot = plot.Width / Values.Count;
        canvas.FontColor = AxisColor;
        canvas.FontSize = 12;
        canvas.FillColor = Color;
        canvas.StrokeColor = Color;
        canvas.StrokeSize = 3;
        canvas.StrokeLineJoin = LineJoin.Round;

        var line = IsLine ? new PathF() : null;
        for (var i = 0; i < Values.Count; i++)
        {
            var x = plot.Left + (slot * i);
            var height = (float)(Math.Clamp(Values[i] / Maximum, 0, 1) * plot.Height);
            canvas.DrawString(Days[i % Days.Length], x, plot.Bottom, slot, LabelHeight, HorizontalAlignment.Center, VerticalAlignment.Center);

            if (line is null)
            {
                canvas.FillRoundedRectangle(x + (slot * 0.2f), plot.Bottom - height, slot * 0.6f, height, 6);
            }
            else if (i == 0)
            {
                line.MoveTo(x + (slot / 2), plot.Bottom - height);
            }
            else
            {
                line.LineTo(x + (slot / 2), plot.Bottom - height);
            }
        }

        if (line is not null)
        {
            canvas.DrawPath(line);
            foreach (var point in line.Points)
            {
                canvas.FillCircle(point, 5);
            }
        }
    }
}
