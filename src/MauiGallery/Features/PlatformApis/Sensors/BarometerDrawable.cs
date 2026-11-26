namespace MauiGallery.Features.PlatformApis;

/// <summary>A classic weather-glass dial from 960 to 1060 hPa, with a needle at the current pressure.</summary>
public sealed class BarometerDrawable(BarometerViewModel viewModel) : IDrawable
{
    private const double Minimum = 960;
    private const double Maximum = 1060;

    private static readonly Color DialColor = Color.FromArgb("#8E8E93");
    private static readonly Color NeedleColor = Color.FromArgb("#3B82F6");
    private static readonly string[] Weather = ["Stormy", "Rain", "Change", "Fair", "Dry"];

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        // A half circle, from 180° on the left to 0° on the right, with its center near the bottom.
        var radius = Math.Min(dirtyRect.Width / 2, dirtyRect.Height - 16) - 8;
        var center = new PointF(dirtyRect.Center.X, dirtyRect.Top + radius + 8);

        canvas.StrokeColor = DialColor.WithAlpha(0.3f);
        canvas.StrokeSize = 10;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.DrawArc(center.X - radius, center.Y - radius, radius * 2, radius * 2, 180, 0, clockwise: true, closed: false);

        canvas.Font = Microsoft.Maui.Graphics.Font.Default;
        canvas.FontSize = 11;
        canvas.FontColor = DialColor;
        for (var i = 0; i < Weather.Length; i++)
        {
            var angle = Math.PI * (1 - ((i + 0.5) / Weather.Length));
            var (sin, cos) = Math.SinCos(angle);
            var distance = radius - 26;
            canvas.DrawString(Weather[i], center.X + (float)(cos * distance) - 30, center.Y - (float)(sin * distance) - 8, 60, 16,
                HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        var fraction = Math.Clamp((viewModel.Pressure - Minimum) / (Maximum - Minimum), 0, 1);
        var (needleSin, needleCos) = Math.SinCos(Math.PI * (1 - fraction));
        var length = radius - 8;
        canvas.StrokeColor = NeedleColor;
        canvas.StrokeSize = 4;
        canvas.DrawLine(center.X, center.Y, center.X + (float)(needleCos * length), center.Y - (float)(needleSin * length));
        canvas.FillColor = NeedleColor;
        canvas.FillCircle(center, 7);
    }
}
