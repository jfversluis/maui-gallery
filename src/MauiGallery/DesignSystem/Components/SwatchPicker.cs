namespace MauiGallery.DesignSystem.Components;

/// <summary>An options-pane control for choosing one of a few colors.</summary>
public sealed class SwatchPicker : HorizontalStackLayout
{
    public static readonly BindableProperty SelectedColorProperty =
        BindableProperty.Create(nameof(SelectedColor), typeof(Color), typeof(SwatchPicker), defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, _, _) => ((SwatchPicker)bindable).UpdateSelection());

    private static readonly (string Name, Color Color)[] Swatches =
    [
        ("Purple", Color.FromArgb("#512BD4")),
        ("Blue", Color.FromArgb("#2563EB")),
        ("Teal", Color.FromArgb("#0D9488")),
        ("Orange", Color.FromArgb("#EA580C")),
        ("Pink", Color.FromArgb("#DB2777")),
        ("Graphite", Color.FromArgb("#374151")),
    ];

    public SwatchPicker()
    {
        Spacing = 8;
        foreach (var (name, color) in Swatches)
        {
            var swatch = new Border
            {
                WidthRequest = 28,
                HeightRequest = 28,
                BackgroundColor = color,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.Ellipse(),
                StrokeThickness = 0,
                AutomationId = $"swatch-{name.ToLowerInvariant()}",
            };
            SemanticProperties.SetDescription(swatch, name);
            ToolTipProperties.SetText(swatch, name);
            swatch.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => SelectedColor = color) });
            Children.Add(swatch);
        }
    }

    public Color? SelectedColor
    {
        get => (Color?)GetValue(SelectedColorProperty);
        set => SetValue(SelectedColorProperty, value);
    }

    private void UpdateSelection()
    {
        foreach (var swatch in Children.OfType<Border>())
        {
            var selected = swatch.BackgroundColor.Equals(SelectedColor);
            swatch.StrokeThickness = selected ? 3 : 0;
            swatch.SetDynamicResource(Border.StrokeProperty, "BrushOutlineStrong");
            swatch.Scale = selected ? 1.1 : 1;
        }
    }
}
