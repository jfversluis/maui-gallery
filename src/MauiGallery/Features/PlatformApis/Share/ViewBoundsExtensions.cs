namespace MauiGallery.Features.PlatformApis;

/// <summary>Finds where a view is on screen, to anchor share sheets and other popovers to it.</summary>
public static class ViewBoundsExtensions
{
    /// <summary>
    /// The view's bounds in its window, which iPad and Mac share sheets point their popover at. Other platforms
    /// don't use popovers, so they get <see cref="Rect.Zero"/>.
    /// </summary>
    public static Rect GetBoundsInWindow(this View view)
    {
#if IOS || MACCATALYST
        if (view.Handler?.PlatformView is UIKit.UIView platformView)
        {
            var bounds = platformView.ConvertRectToView(platformView.Bounds, null);
            return new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height);
        }
#endif
        return Rect.Zero;
    }
}
