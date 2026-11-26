#if IOS || MACCATALYST
using System.Runtime.CompilerServices;
#endif

namespace MauiGallery.Features.Samples;

/// <summary>
/// Stops the scroll views around a view from taking over a drag that starts on it, so a sketchpad on a scrolling
/// page draws instead of scrolling. Use <see cref="CaptureDrags"/>, or for a view that reports its own touches, call
/// <see cref="Attach"/> once and <see cref="Begin"/> and <see cref="End"/> when a touch starts and ends.
/// </summary>
internal static class ScrollCapture
{
#if IOS || MACCATALYST
    // The scroll views each view disabled, kept per view so views in different windows don't interfere.
    private static readonly ConditionalWeakTable<View, List<UIKit.UIScrollView>> Disabled = new();

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

    /// <summary>
    /// Keeps the scroll views around <paramref name="view"/> still for every touch that starts on it, so its pan,
    /// pinch or swipe gesture recognizers get the whole drag.
    /// </summary>
    public static void CaptureDrags(View view)
    {
        Attach(view);
        var pointer = new PointerGestureRecognizer();
        pointer.PointerPressed += (_, _) => Begin(view);
        pointer.PointerReleased += (_, _) => End(view);

        // On iOS and Mac, a pressed pointer that leaves the view raises PointerExited instead of PointerReleased.
        pointer.PointerExited += (_, _) => End(view);
        view.GestureRecognizers.Add(pointer);
    }

    /// <summary>Prepares the scroll views around <paramref name="view"/> when it loads, and lets them go when it unloads.</summary>
    public static void Attach(View view)
    {
        view.Loaded += (_, _) => DeliverTouchesAtOnce(view);
        view.Unloaded += (_, _) => End(view);
        if (view.IsLoaded)
        {
            DeliverTouchesAtOnce(view);
        }
    }

    public static void Begin(View view)
    {
#if ANDROID
        // Asks every parent, including the ScrollView, not to intercept the rest of this gesture.
        (view.Handler?.PlatformView as Android.Views.View)?.Parent?.RequestDisallowInterceptTouchEvent(true);
#elif IOS || MACCATALYST
        if (Disabled.TryGetValue(view, out _))
        {
            return;
        }

        // Disabling scrolling stops the scroll view's pan gesture from cancelling the touch.
        var disabled = new List<UIKit.UIScrollView>();
        foreach (var scrollView in ScrollViewsAround(view))
        {
            if (scrollView.ScrollEnabled)
            {
                scrollView.ScrollEnabled = false;
                disabled.Add(scrollView);
            }
        }

        Disabled.Add(view, disabled);
#endif
    }

    public static void End(View view)
    {
#if IOS || MACCATALYST
        if (!Disabled.TryGetValue(view, out var disabled))
        {
            return;
        }

        Disabled.Remove(view);
        foreach (var scrollView in disabled)
        {
            scrollView.ScrollEnabled = true;
        }
#endif
    }

    private static void DeliverTouchesAtOnce(View view)
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
}
