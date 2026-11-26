#if ANDROID
using Android.Widget;
using Microsoft.Maui.Handlers;

namespace MauiGallery.Features.NativePower;

public sealed partial class NativeCalendarHandler : ViewHandler<NativeCalendar, CalendarView>
{
    protected override CalendarView CreatePlatformView() => new(Context);

    protected override void ConnectHandler(CalendarView platformView)
    {
        base.ConnectHandler(platformView);
        platformView.DateChange += OnDateChange;
    }

    protected override void DisconnectHandler(CalendarView platformView)
    {
        platformView.DateChange -= OnDateChange;
        base.DisconnectHandler(platformView);
    }

    private static void MapSelectedDate(NativeCalendarHandler handler, NativeCalendar calendar)
    {
        var millis = ToMillis(calendar.SelectedDate);
        if (handler.PlatformView.Date != millis)
        {
            handler.PlatformView.Date = millis;
        }
    }

    private static void MapDateRange(NativeCalendarHandler handler, NativeCalendar calendar)
    {
        // CalendarView rejects a minimum after its maximum, so widen the range before narrowing it.
        handler.PlatformView.MinDate = ToMillis(EarliestDate);
        handler.PlatformView.MaxDate = ToMillis(LatestDate);
        handler.PlatformView.MinDate = ToMillis(calendar.MinimumDate ?? EarliestDate);
        handler.PlatformView.MaxDate = ToMillis(calendar.MaximumDate ?? LatestDate);
    }

    private static void MapAnimateTo(NativeCalendarHandler handler, NativeCalendar calendar, object? args)
    {
        if (args is DateTime date)
        {
            handler.PlatformView.SetDate(ToMillis(date), animate: true, center: true);
            calendar.SelectedDate = date;
        }
    }

    // CalendarView works in milliseconds since 1970, in the device's time zone.
    private static long ToMillis(DateTime date) =>
        new DateTimeOffset(DateTime.SpecifyKind(date.Date, DateTimeKind.Local)).ToUnixTimeMilliseconds();

    private void OnDateChange(object? sender, CalendarView.DateChangeEventArgs e) =>
        VirtualView?.SelectedDate = new DateTime(e.Year, e.Month + 1, e.DayOfMonth);
}
#endif
