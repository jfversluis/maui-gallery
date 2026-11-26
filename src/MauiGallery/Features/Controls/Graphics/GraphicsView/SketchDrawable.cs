namespace MauiGallery.Features.Controls.Graphics;

/// <summary>Keeps the strokes drawn with a finger, mouse or pen, and draws them.</summary>
public sealed class SketchDrawable : IDrawable
{
    private readonly List<(PathF Path, Color Color, float Size)> _strokes = [];

    public Color InkColor { get; set; } = Color.FromArgb("#2563EB");

    public float InkSize { get; set; } = 6;

    public int StrokeCount => _strokes.Count;

    public void StartStroke(PointF point)
    {
        var path = new PathF(point);
        path.LineTo(point); // A tap still draws a dot.
        _strokes.Add((path, InkColor, InkSize));
    }

    public void ContinueStroke(PointF point)
    {
        if (_strokes.Count > 0)
        {
            _strokes[^1].Path.LineTo(point);
        }
    }

    public void Clear() => _strokes.Clear();

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeLineJoin = LineJoin.Round;
        foreach (var (path, color, size) in _strokes)
        {
            canvas.StrokeColor = color;
            canvas.StrokeSize = size;
            canvas.DrawPath(path);
        }
    }
}
