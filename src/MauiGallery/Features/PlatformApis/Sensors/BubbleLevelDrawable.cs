using System.Numerics;

namespace MauiGallery.Features.PlatformApis;

/// <summary>A spirit level: the bubble floats to the side of the device that's highest.</summary>
public sealed class BubbleLevelDrawable(AccelerometerViewModel viewModel) : IDrawable
{
    private static readonly Color RingColor = Color.FromArgb("#8E8E93");
    private static readonly Color LevelColor = Color.FromArgb("#22C55E");
    private static readonly Color TiltedColor = Color.FromArgb("#F59E0B");

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var center = dirtyRect.Center;
        var radius = (Math.Min(dirtyRect.Width, dirtyRect.Height) / 2) - 2;
        var bubbleRadius = radius * 0.18f;

        canvas.FillColor = RingColor.WithAlpha(0.12f);
        canvas.FillCircle(center, radius);
        canvas.StrokeColor = RingColor;
        canvas.StrokeSize = 2;
        canvas.DrawCircle(center, radius);
        canvas.DrawCircle(center, bubbleRadius + 4);
        canvas.StrokeSize = 1;
        canvas.DrawLine(center.X - radius, center.Y, center.X + radius, center.Y);
        canvas.DrawLine(center.X, center.Y - radius, center.X, center.Y + radius);

        // X and Y are the share of gravity along each axis: 0 when the device lies flat, ±1 when it stands on an edge.
        // The reading points away from the ground, so lifting the left edge makes X negative; the bubble goes left.
        var acceleration = viewModel.Acceleration;
        var offset = new Vector2(acceleration.X, -acceleration.Y);
        var travel = radius - bubbleRadius - 2;
        if (offset.Length() > 1)
        {
            offset = Vector2.Normalize(offset);
        }

        var isLevel = offset.Length() < 0.05;
        canvas.FillColor = (isLevel ? LevelColor : TiltedColor).WithAlpha(0.85f);
        canvas.FillCircle(center.X + (offset.X * travel), center.Y + (offset.Y * travel), bubbleRadius);
    }
}
