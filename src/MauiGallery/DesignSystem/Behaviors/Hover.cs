namespace MauiGallery.DesignSystem.Behaviors;

/// <summary>
/// Pointer-hover background for chrome elements on desktop. <c>Hover.Background</c> names a color token; while the
/// pointer is over the element that token becomes its background, and the styled value returns afterwards.
/// </summary>
/// <remarks>
/// Visual states declared in a <see cref="Style"/> keep the first value of a <c>DynamicResource</c> setter, so they
/// would not follow a theme switch; the token is looked up each time the pointer enters instead. The color is set as a
/// local value because <see cref="BindableObject.ClearValue(BindableProperty)"/> only removes local values: a value
/// set with <c>SetDynamicResource</c> would stay after the pointer leaves.
/// </remarks>
public static class Hover
{
    public static readonly BindableProperty BackgroundProperty =
        BindableProperty.CreateAttached("Background", typeof(string), typeof(Hover), null, propertyChanged: OnBackgroundChanged);

    private static readonly BindableProperty RecognizerProperty =
        BindableProperty.CreateAttached("Recognizer", typeof(PointerGestureRecognizer), typeof(Hover), null);

    public static string? GetBackground(BindableObject view) => (string?)view.GetValue(BackgroundProperty);

    public static void SetBackground(BindableObject view, string? value) => view.SetValue(BackgroundProperty, value);

    private static void OnBackgroundChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        if (bindable is not View view)
        {
            return;
        }

        if (view.GetValue(RecognizerProperty) is PointerGestureRecognizer existing)
        {
            view.GestureRecognizers.Remove(existing);
            view.ClearValue(RecognizerProperty);
        }

        if (newValue is not string key)
        {
            return;
        }

        var recognizer = new PointerGestureRecognizer();
        recognizer.PointerEntered += (_, _) =>
        {
            if (Application.Current?.Resources.TryGetValue(key, out var value) == true && value is Color color)
            {
                view.BackgroundColor = color;
            }
        };
        recognizer.PointerExited += (_, _) => view.ClearValue(VisualElement.BackgroundColorProperty);
        view.GestureRecognizers.Add(recognizer);
        view.SetValue(RecognizerProperty, recognizer);
    }
}
