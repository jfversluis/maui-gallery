namespace MauiGallery.Features.Controls.Layouts;

/// <summary>A row of tappable stars for the ContentView sample. <see cref="Value"/> binds two-way by default.</summary>
public sealed class RatingView : ContentView
{
    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(int), typeof(RatingView), 0, BindingMode.TwoWay,
            coerceValue: (bindable, value) => Math.Clamp((int)value, 0, ((RatingView)bindable).Maximum),
            propertyChanged: (bindable, _, _) => ((RatingView)bindable).UpdateStars());

    public static readonly BindableProperty MaximumProperty =
        BindableProperty.Create(nameof(Maximum), typeof(int), typeof(RatingView), 5,
            validateValue: (_, value) => (int)value > 0,
            propertyChanged: (bindable, _, _) => ((RatingView)bindable).CreateStars());

    public static readonly BindableProperty ColorProperty =
        BindableProperty.Create(nameof(Color), typeof(Color), typeof(RatingView), Colors.Orange,
            propertyChanged: (bindable, _, _) => ((RatingView)bindable).UpdateStars());

    private readonly HorizontalStackLayout _stars = new() { Spacing = 4 };

    public RatingView()
    {
        Content = _stars;
        CreateStars();
    }

    public int Value
    {
        get => (int)GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public int Maximum
    {
        get => (int)GetValue(MaximumProperty);
        set => SetValue(MaximumProperty, value);
    }

    public Color Color
    {
        get => (Color)GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    private void CreateStars()
    {
        _stars.Clear();
        for (var i = 1; i <= Maximum; i++)
        {
            var rating = i;
            var star = new Label { Text = "★", FontSize = 32, AutomationId = $"sample-rating-star-{rating}" };
            SemanticProperties.SetDescription(star, rating == 1 ? "1 star" : $"{rating} stars");

            // Tapping the current rating again clears it.
            star.GestureRecognizers.Add(new TapGestureRecognizer { Command = new Command(() => Value = Value == rating ? 0 : rating) });
            _stars.Add(star);
        }

        if (Value > Maximum)
        {
            Value = Maximum;
        }

        UpdateStars();
    }

    private void UpdateStars()
    {
        for (var i = 0; i < _stars.Count; i++)
        {
            ((Label)_stars[i]).TextColor = i < Value ? Color : Colors.Gray.WithAlpha(0.4f);
        }
    }
}
