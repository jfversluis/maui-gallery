namespace MauiGallery.Features.Controls.Graphics;

/// <summary>Draws a 270 degree gauge with the value in the middle.</summary>
public sealed class GaugeDrawable : IDrawable
{
    // Angles are in degrees, counterclockwise from 3 o'clock: the gauge runs clockwise from bottom-left to bottom-right.
    private const float StartAngle = 225;
    private const float Sweep = 270;

    public double Value { get; set; }

    public Color Color { get; set; } = Color.FromArgb("#0D9488");

    public float Thickness { get; set; } = 16;

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var size = Math.Min(dirtyRect.Width, dirtyRect.Height) - Thickness;
        var x = dirtyRect.Center.X - (size / 2);
        var y = dirtyRect.Center.Y - (size / 2);

        canvas.StrokeSize = Thickness;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeColor = Color.WithAlpha(0.2f);
        canvas.DrawArc(x, y, size, size, StartAngle, StartAngle - Sweep, clockwise: true, closed: false);

        var fraction = (float)Math.Clamp(Value / 100, 0, 1);
        if (fraction > 0)
        {
            canvas.StrokeColor = Color;
            canvas.DrawArc(x, y, size, size, StartAngle, StartAngle - (Sweep * fraction), clockwise: true, closed: false);
        }

        canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
        canvas.FontSize = size / 4;
        canvas.FontColor = Color;
        canvas.DrawString($"{Value:0}%", dirtyRect, HorizontalAlignment.Center, VerticalAlignment.Center);
    }
}
