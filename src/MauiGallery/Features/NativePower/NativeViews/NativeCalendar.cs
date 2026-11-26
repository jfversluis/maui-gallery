namespace MauiGallery.Features.NativePower;

/// <summary>
/// A month calendar drawn by the platform: an inline <c>UIDatePicker</c> on iOS and Mac Catalyst, a
/// <c>CalendarView</c> on Android and Windows. Rendered by <see cref="NativeCalendarHandler"/>.
/// </summary>
public sealed class NativeCalendar : View
{
    public static readonly BindableProperty SelectedDateProperty =
        BindableProperty.Create(nameof(SelectedDate), typeof(DateTime), typeof(NativeCalendar), default(DateTime), BindingMode.TwoWay,
            coerceValue: (bindable, value) => ((NativeCalendar)bindable).Clamp((DateTime)value),
            propertyChanged: (bindable, oldValue, newValue) =>
                ((NativeCalendar)bindable).DateSelected?.Invoke(bindable, new DateChangedEventArgs((DateTime)oldValue, (DateTime)newValue)));

    public static readonly BindableProperty MinimumDateProperty =
        BindableProperty.Create(nameof(MinimumDate), typeof(DateTime?), typeof(NativeCalendar),
            propertyChanged: (bindable, _, _) => bindable.CoerceValue(SelectedDateProperty));

    public static readonly BindableProperty MaximumDateProperty =
        BindableProperty.Create(nameof(MaximumDate), typeof(DateTime?), typeof(NativeCalendar),
            propertyChanged: (bindable, _, _) => bindable.CoerceValue(SelectedDateProperty));

    public NativeCalendar() => SelectedDate = DateTime.Today;

    /// <summary>Raised when the selected date changes, from a tap or from code.</summary>
    public event EventHandler<DateChangedEventArgs>? DateSelected;

    public DateTime SelectedDate
    {
        get => (DateTime)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    public DateTime? MinimumDate
    {
        get => (DateTime?)GetValue(MinimumDateProperty);
        set => SetValue(MinimumDateProperty, value);
    }

    public DateTime? MaximumDate
    {
        get => (DateTime?)GetValue(MaximumDateProperty);
        set => SetValue(MaximumDateProperty, value);
    }

    /// <summary>
    /// Selects <paramref name="date"/> and scrolls the calendar to it with the platform's animation. Setting
    /// <see cref="SelectedDate"/> jumps there instead. This is an action rather than state, so it's a command.
    /// </summary>
    public void AnimateTo(DateTime date) => Handler?.Invoke(nameof(AnimateTo), Clamp(date.Date));

    private DateTime Clamp(DateTime date)
    {
        date = date.Date;
        if (MinimumDate is { } min && date < min.Date)
        {
            return min.Date;
        }

        return MaximumDate is { } max && date > max.Date ? max.Date : date;
    }
}
