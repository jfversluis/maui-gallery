namespace MauiGallery.Features.NativePower;

/// <summary>
/// Connects <see cref="NativeCalendar"/> to a native calendar. This file holds the mappers; each platform's
/// <c>NativeCalendarHandler.&lt;Platform&gt;.cs</c> creates the native view and implements the map methods.
/// </summary>
public sealed partial class NativeCalendarHandler
{
    /// <summary>Properties: when one changes on the <see cref="NativeCalendar"/>, its map method updates the native view.</summary>
    public static readonly IPropertyMapper<NativeCalendar, NativeCalendarHandler> Mapper =
        new PropertyMapper<NativeCalendar, NativeCalendarHandler>(ViewMapper)
        {
            // The range comes first, so the selected date is always inside it.
            [nameof(NativeCalendar.MinimumDate)] = MapDateRange,
            [nameof(NativeCalendar.MaximumDate)] = MapDateRange,
            [nameof(NativeCalendar.SelectedDate)] = MapSelectedDate,
        };

    /// <summary>Commands: actions the <see cref="NativeCalendar"/> asks for with <c>Handler.Invoke</c>.</summary>
    public static readonly CommandMapper<NativeCalendar, NativeCalendarHandler> CommandMapper =
        new(ViewCommandMapper)
        {
            [nameof(NativeCalendar.AnimateTo)] = MapAnimateTo,
        };

    public NativeCalendarHandler()
        : base(Mapper, CommandMapper)
    {
    }

    // The earliest and latest dates the calendars show when no range is set.
    private static DateTime EarliestDate => DateTime.Today.AddYears(-100);

    private static DateTime LatestDate => DateTime.Today.AddYears(100);
}
