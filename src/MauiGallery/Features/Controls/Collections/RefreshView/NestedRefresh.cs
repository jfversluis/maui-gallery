namespace MauiGallery.Features.Controls.Collections;

/// <summary>
/// Keeps a pull inside a <see cref="RefreshView"/> that sits in a scrolling page. On Android the pull otherwise goes
/// to the page first: the page scrolls up and the RefreshView only starts refreshing once the page reaches its top.
/// Every sample in the gallery is in a page ScrollView; apps rarely nest a RefreshView like that.
/// </summary>
internal static class NestedRefresh
{
    public static void KeepPullInside(RefreshView refreshView)
    {
#if ANDROID
        refreshView.Loaded += OnLoaded;

        static void OnLoaded(object? sender, EventArgs e)
        {
            var refreshView = (RefreshView)sender!;
            refreshView.Loaded -= OnLoaded;
            if (refreshView.Handler?.PlatformView is not AndroidX.SwipeRefreshLayout.Widget.SwipeRefreshLayout layout ||
                refreshView.Content?.Handler?.PlatformView is not Android.Views.View content)
            {
                return;
            }

            // Don't offer the content's overscroll to the page before the refresh spinner.
            layout.NestedScrollingEnabled = false;

            // And stop the page from intercepting the drag; it would otherwise take over every vertical drag.
            void KeepTouch() => layout.Parent?.RequestDisallowInterceptTouchEvent(true);
            if (content is AndroidX.RecyclerView.Widget.RecyclerView list)
            {
                list.AddOnItemTouchListener(new TouchDownListener(KeepTouch));
            }
            else
            {
                content.Touch += (_, args) =>
                {
                    if (args.Event?.ActionMasked == Android.Views.MotionEventActions.Down)
                    {
                        KeepTouch();
                    }

                    args.Handled = false;
                };
            }
        }
#endif
    }

#if ANDROID
    private sealed class TouchDownListener(Action onTouchDown) : AndroidX.RecyclerView.Widget.RecyclerView.SimpleOnItemTouchListener
    {
        public override bool OnInterceptTouchEvent(AndroidX.RecyclerView.Widget.RecyclerView view, Android.Views.MotionEvent e)
        {
            if (e.ActionMasked == Android.Views.MotionEventActions.Down)
            {
                onTouchDown();
            }

            return false;
        }
    }
#endif
}
