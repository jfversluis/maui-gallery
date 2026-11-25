namespace MauiGallery.DesignSystem.Components;

/// <summary>Draws the subtle dot grid behind samples, so the stage reads as a canvas rather than app chrome.</summary>
public sealed class DotGridView : GraphicsView
{
    public static readonly BindableProperty DotColorProperty =
        BindableProperty.Create(nameof(DotColor), typeof(Color), typeof(DotGridView), Colors.LightGray,
            propertyChanged: (bindable, _, _) => ((DotGridView)bindable).Invalidate());

    public DotGridView()
    {
        Drawable = new DotGridDrawable(this);
        InputTransparent = true;
        SemanticProperties.SetDescription(this, string.Empty);
        AutomationProperties.SetIsInAccessibleTree(this, false);
    }

    public Color DotColor
    {
        get => (Color)GetValue(DotColorProperty);
        set => SetValue(DotColorProperty, value);
    }

    private sealed class DotGridDrawable(DotGridView owner) : IDrawable
    {
        private const float Spacing = 16;
        private const float Radius = 1;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            canvas.FillColor = owner.DotColor;
            for (var y = Spacing / 2; y < dirtyRect.Height; y += Spacing)
            {
                for (var x = Spacing / 2; x < dirtyRect.Width; x += Spacing)
                {
                    canvas.FillCircle(x, y, Radius);
                }
            }
        }
    }
}
