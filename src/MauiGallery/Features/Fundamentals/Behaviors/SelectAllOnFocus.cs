namespace MauiGallery.Features.Fundamentals;

/// <summary>
/// An attached behavior: setting <c>SelectAllOnFocus.IsEnabled</c> to true selects an input's text when it gets focus.
/// </summary>
public static class SelectAllOnFocus
{
    public static readonly BindableProperty IsEnabledProperty =
        BindableProperty.CreateAttached("IsEnabled", typeof(bool), typeof(SelectAllOnFocus), false, propertyChanged: OnIsEnabledChanged);

    public static bool GetIsEnabled(BindableObject view) => (bool)view.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(BindableObject view, bool value) => view.SetValue(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not InputView input)
        {
            return;
        }

        input.Focused -= OnFocused;
        if ((bool)newValue)
        {
            input.Focused += OnFocused;
        }
    }

    private static void OnFocused(object? sender, FocusEventArgs e)
    {
        var input = (InputView)sender!;

        // Let the platform finish placing the caret first, or it replaces the selection.
        input.Dispatcher.Dispatch(() =>
        {
            input.CursorPosition = 0;
            input.SelectionLength = input.Text?.Length ?? 0;
        });
    }
}
