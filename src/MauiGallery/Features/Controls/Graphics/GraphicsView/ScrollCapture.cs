namespace MauiGallery.Features.Controls.Graphics;

/// <summary>
/// Stops the scroll views around a view from taking over a drag that starts on it, so a sketchpad on a scrolling
/// page draws instead of scrolling. Call <see cref="Attach"/> once the view is loaded, <see cref="Begin"/> when a
/// touch starts and <see cref="End"/> when it ends.
/// </summary>
internal static class ScrollCapture
{
#if IOS || MACCATALYST
    private static readonly List<UIKit.UIScrollView> Disabled = [];

    private static IEnumerable<UIKit.UIScrollView> ScrollViewsAround(View view)
    {
        for (var parent = (view.Handler?.PlatformView as UIKit.UIView)?.Superview; parent is not null; parent = parent.Superview)
        {
            if (parent is UIKit.UIScrollView scrollView)
            {
                yield return scrollView;
            }
        }
    }
#endif

    public static void Attach(View view)
    {
#if IOS || MACCATALYST
        // A UIScrollView holds a touch back for a moment to see whether it becomes a scroll, and a vertical drag
        // does. Deliver touches at once instead, so Begin runs before the scroll view's pan gesture starts.
        foreach (var scrollView in ScrollViewsAround(view))
        {
            scrollView.DelaysContentTouches = false;
        }
#endif
    }

    public static void Begin(View view)
    {
#if ANDROID
        // Asks every parent, including the ScrollView, not to intercept the rest of this gesture.
        (view.Handler?.PlatformView as Android.Views.View)?.Parent?.RequestDisallowInterceptTouchEvent(true);
#elif IOS || MACCATALYST
        // Disabling scrolling stops the scroll view's pan gesture from cancelling the touch.
        foreach (var scrollView in ScrollViewsAround(view))
        {
            if (scrollView.ScrollEnabled)
            {
                scrollView.ScrollEnabled = false;
                Disabled.Add(scrollView);
            }
        }
#endif
    }

    public static void End()
    {
#if IOS || MACCATALYST
        foreach (var scrollView in Disabled)
        {
            scrollView.ScrollEnabled = true;
        }

        Disabled.Clear();
#endif
    }
}
