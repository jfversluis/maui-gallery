#if IOS || MACCATALYST
using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using UIKit;

namespace MauiGallery.Features.NativePower;

public sealed partial class NativeCalendarHandler : ViewHandler<NativeCalendar, UIDatePicker>
{
    protected override UIDatePicker CreatePlatformView() => new()
    {
        Mode = UIDatePickerMode.Date,
        PreferredDatePickerStyle = UIDatePickerStyle.Inline,

        // ToNSDate and ToDateTime don't convert time zones, so read and write the dates in UTC.
        TimeZone = NSTimeZone.FromName("UTC"),
    };

    protected override void ConnectHandler(UIDatePicker platformView)
    {
        base.ConnectHandler(platformView);
        platformView.ValueChanged += OnValueChanged;
    }

    protected override void DisconnectHandler(UIDatePicker platformView)
    {
        platformView.ValueChanged -= OnValueChanged;
        base.DisconnectHandler(platformView);
    }

    private static void MapSelectedDate(NativeCalendarHandler handler, NativeCalendar calendar)
    {
        // Skip dates the picker already shows, such as one the user just tapped or one AnimateTo is animating to.
        if (handler.PlatformView.Date.ToDateTime().Date != calendar.SelectedDate)
        {
            handler.PlatformView.SetDate(calendar.SelectedDate.ToNSDate(), animated: false);
        }
    }

    private static void MapDateRange(NativeCalendarHandler handler, NativeCalendar calendar)
    {
        handler.PlatformView.MinimumDate = calendar.MinimumDate?.ToNSDate();
        handler.PlatformView.MaximumDate = calendar.MaximumDate?.ToNSDate();
    }

    private static void MapAnimateTo(NativeCalendarHandler handler, NativeCalendar calendar, object? args)
    {
        if (args is DateTime date)
        {
            handler.PlatformView.SetDate(date.ToNSDate(), animated: true);
            calendar.SelectedDate = date;
        }
    }

    // UIKit raises ValueChanged only for the user's taps, not for SetDate.
    private void OnValueChanged(object? sender, EventArgs e) =>
        VirtualView?.SelectedDate = PlatformView.Date.ToDateTime().Date;
}
#endif
