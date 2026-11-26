namespace MauiGallery.DesignSystem.Components;

/// <summary>A small rounded label for metadata such as native mappings, platforms and "New" markers.</summary>
public partial class Chip : ContentView
{
    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(Chip), string.Empty);

    public static readonly BindableProperty GlyphProperty =
        BindableProperty.Create(nameof(Glyph), typeof(string), typeof(Chip));

    public static readonly BindableProperty IsAccentProperty =
        BindableProperty.Create(nameof(IsAccent), typeof(bool), typeof(Chip), false, propertyChanged: (b, _, _) => ((Chip)b).ApplyAccent());

    public Chip()
    {
        InitializeComponent();
        ApplyAccent();
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public string? Glyph
    {
        get => (string?)GetValue(GlyphProperty);
        set => SetValue(GlyphProperty, value);
    }

    public bool IsAccent
    {
        get => (bool)GetValue(IsAccentProperty);
        set => SetValue(IsAccentProperty, value);
    }

    /// <summary>
    /// The row inside the pill measures its text without a width limit, so cap the text to the space the chip gets:
    /// long text (such as a native mapping) then wraps instead of running past the edge of a phone screen.
    /// </summary>
    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        var maxTextWidth = double.PositiveInfinity;
        if (double.IsFinite(widthConstraint))
        {
            var glyphWidth = GlyphLabel.IsVisible ? GlyphLabel.Measure(double.PositiveInfinity, double.PositiveInfinity).Width + 6 : 0;
            maxTextWidth = Math.Max(0, widthConstraint - Margin.HorizontalThickness - Pill.Padding.HorizontalThickness - glyphWidth);
        }

        if (!TextLabel.MaximumWidthRequest.Equals(maxTextWidth))
        {
            TextLabel.MaximumWidthRequest = maxTextWidth;
        }

        return base.MeasureOverride(widthConstraint, heightConstraint);
    }

    private void ApplyAccent()
    {
        Pill.SetDynamicResource(BackgroundColorProperty, IsAccent ? "ColorAccentSubtle" : "ColorSurfaceAlt");
        TextLabel.SetDynamicResource(Label.TextColorProperty, IsAccent ? "ColorAccentText" : "ColorTextPrimary");
        GlyphLabel.SetDynamicResource(Label.TextColorProperty, IsAccent ? "ColorAccentText" : "ColorTextSecondary");
    }
}
