using System.Numerics;

namespace MauiGallery.Features.PlatformApis;

/// <summary>Three bars, one per axis, that grow left or right of zero with a sensor's X, Y and Z values.</summary>
public sealed class AxisBarsDrawable(Func<Vector3> read, float range) : IDrawable
{
    private static readonly Color LabelColor = Color.FromArgb("#8E8E93");
    private static readonly Color[] AxisColors = [Color.FromArgb("#EF4444"), Color.FromArgb("#22C55E"), Color.FromArgb("#3B82F6")];
    private static readonly string[] AxisNames = ["X", "Y", "Z"];

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        const float LabelWidth = 24;
        const float BarHeight = 18;

        var value = read();
        float[] values = [value.X, value.Y, value.Z];
        var trackLeft = dirtyRect.Left + LabelWidth;
        var trackWidth = dirtyRect.Width - LabelWidth;
        var zero = trackLeft + (trackWidth / 2);
        var rowHeight = dirtyRect.Height / 3;

        canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
        canvas.FontSize = 14;
        canvas.FontColor = LabelColor;

        for (var i = 0; i < 3; i++)
        {
            var top = dirtyRect.Top + (i * rowHeight) + ((rowHeight - BarHeight) / 2);
            canvas.DrawString(AxisNames[i], dirtyRect.Left, top, LabelWidth, BarHeight, HorizontalAlignment.Left, VerticalAlignment.Center);

            canvas.FillColor = LabelColor.WithAlpha(0.15f);
            canvas.FillRoundedRectangle(trackLeft, top, trackWidth, BarHeight, BarHeight / 2);

            var length = Math.Clamp(values[i] / range, -1, 1) * (trackWidth / 2);
            canvas.FillColor = AxisColors[i];
            canvas.FillRoundedRectangle(Math.Min(zero, zero + length), top, Math.Max(Math.Abs(length), 2), BarHeight, BarHeight / 2);
        }

        canvas.StrokeColor = LabelColor;
        canvas.StrokeSize = 1;
        canvas.DrawLine(zero, dirtyRect.Top, zero, dirtyRect.Bottom);
    }
}
