#if WINDOWS
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml.Controls;
using WCalendarView = Microsoft.UI.Xaml.Controls.CalendarView;

namespace MauiGallery.Features.NativePower;

public sealed partial class NativeCalendarHandler : ViewHandler<NativeCalendar, WCalendarView>
{
    protected override WCalendarView CreatePlatformView() => new()
    {
        SelectionMode = CalendarViewSelectionMode.Single,
    };

    protected override void ConnectHandler(WCalendarView platformView)
    {
        base.ConnectHandler(platformView);
        platformView.SelectedDatesChanged += OnSelectedDatesChanged;
    }

    protected override void DisconnectHandler(WCalendarView platformView)
    {
        platformView.SelectedDatesChanged -= OnSelectedDatesChanged;
        base.DisconnectHandler(platformView);
    }

    private static void MapSelectedDate(NativeCalendarHandler handler, NativeCalendar calendar)
    {
        var dates = handler.PlatformView.SelectedDates;
        if (dates.Count == 1 && dates[0].Date == calendar.SelectedDate)
        {
            return;
        }

        dates.Clear();
        dates.Add(new DateTimeOffset(calendar.SelectedDate));
        handler.PlatformView.SetDisplayDate(new DateTimeOffset(calendar.SelectedDate));
    }

    private static void MapDateRange(NativeCalendarHandler handler, NativeCalendar calendar)
    {
        handler.PlatformView.MinDate = new DateTimeOffset(calendar.MinimumDate ?? EarliestDate);
        handler.PlatformView.MaxDate = new DateTimeOffset(calendar.MaximumDate ?? LatestDate);
    }

    private static void MapAnimateTo(NativeCalendarHandler handler, NativeCalendar calendar, object? args)
    {
        if (args is DateTime date)
        {
            // CalendarView scrolls to the display date with its own animation.
            handler.PlatformView.SetDisplayDate(new DateTimeOffset(date));
            calendar.SelectedDate = date;
        }
    }

    private void OnSelectedDatesChanged(WCalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
    {
        // Clearing the selection raises this event too, with no added dates.
        if (args.AddedDates.Count > 0)
        {
            VirtualView?.SelectedDate = args.AddedDates[0].Date;
        }
    }
}
#endif
