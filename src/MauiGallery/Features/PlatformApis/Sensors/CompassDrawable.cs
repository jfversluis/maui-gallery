namespace MauiGallery.Features.PlatformApis;

/// <summary>A compass rose that turns so its N always points to magnetic north, under a fixed marker for the heading.</summary>
public sealed class CompassDrawable(CompassViewModel viewModel) : IDrawable
{
    private static readonly Color TickColor = Color.FromArgb("#8E8E93");
    private static readonly Color NorthColor = Color.FromArgb("#EF4444");
    private static readonly Color MarkerColor = Color.FromArgb("#F59E0B");
    private static readonly string[] Cardinals = ["N", "E", "S", "W"];

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        var center = dirtyRect.Center;
        var radius = (Math.Min(dirtyRect.Width, dirtyRect.Height) / 2) - 12;

        canvas.FillColor = TickColor.WithAlpha(0.12f);
        canvas.FillCircle(center, radius);

        // Turn the whole rose against the heading. ICanvas rotates clockwise, in degrees, around a point.
        canvas.SaveState();
        canvas.Rotate(-(float)viewModel.Heading, center.X, center.Y);

        canvas.StrokeColor = TickColor;
        canvas.StrokeLineCap = LineCap.Round;
        for (var degrees = 0; degrees < 360; degrees += 10)
        {
            var isMajor = degrees % 30 == 0;
            canvas.StrokeSize = isMajor ? 2 : 1;
            var inner = radius - (isMajor ? 12 : 7);
            var (sin, cos) = Math.SinCos(degrees * Math.PI / 180);
            canvas.DrawLine(
                center.X + (float)(sin * inner), center.Y - (float)(cos * inner),
                center.X + (float)(sin * radius), center.Y - (float)(cos * radius));
        }

        canvas.Font = Microsoft.Maui.Graphics.Font.DefaultBold;
        canvas.FontSize = radius / 5;
        for (var i = 0; i < 4; i++)
        {
            var (sin, cos) = Math.SinCos(i * Math.PI / 2);
            var distance = radius * 0.68f;
            canvas.FontColor = i == 0 ? NorthColor : TickColor;
            var x = center.X + (float)(sin * distance);
            var y = center.Y - (float)(cos * distance);
            canvas.DrawString(Cardinals[i], x - 20, y - 20, 40, 40, HorizontalAlignment.Center, VerticalAlignment.Center);
        }

        // The needle: red half to north, gray half to south.
        var needle = radius * 0.42f;
        var half = radius * 0.07f;
        var north = new PathF();
        north.MoveTo(center.X, center.Y - needle);
        north.LineTo(center.X + half, center.Y);
        north.LineTo(center.X - half, center.Y);
        north.Close();
        canvas.FillColor = NorthColor;
        canvas.FillPath(north);
        var south = new PathF();
        south.MoveTo(center.X, center.Y + needle);
        south.LineTo(center.X + half, center.Y);
        south.LineTo(center.X - half, center.Y);
        south.Close();
        canvas.FillColor = TickColor;
        canvas.FillPath(south);
        canvas.RestoreState();

        // The marker at the top shows the way the device points.
        var marker = new PathF();
        marker.MoveTo(center.X, center.Y - radius + 2);
        marker.LineTo(center.X - 8, center.Y - radius - 10);
        marker.LineTo(center.X + 8, center.Y - radius - 10);
        marker.Close();
        canvas.FillColor = MarkerColor;
        canvas.FillPath(marker);
        canvas.FillCircle(center, 4);
    }
}
