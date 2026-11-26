using Microsoft.Maui.Handlers;
#if IOS || MACCATALYST
using PlatformView = UIKit.UIView;
#elif ANDROID
using PlatformView = Android.Views.View;
#elif WINDOWS
using PlatformView = Microsoft.UI.Xaml.FrameworkElement;
#endif

namespace MauiGallery.Features.NativePower;

/// <summary>
/// Hosts a native view that your code creates, for a control that exists on one platform only. Its handler
/// calls <see cref="Create"/> once, sizes the view like any other, and calls <see cref="Release"/> when it disconnects.
/// </summary>
public sealed class PlatformViewHost : View
{
    /// <summary>Creates the native view. Set it before the host is shown.</summary>
    public Func<IMauiContext, PlatformView>? Create { get; set; }

    /// <summary>Undoes what <see cref="Create"/> did, such as unsubscribing from native events.</summary>
    public Action<PlatformView>? Release { get; set; }
}

public sealed class PlatformViewHostHandler() : ViewHandler<PlatformViewHost, PlatformView>(ViewMapper)
{
    protected override PlatformView CreatePlatformView() =>
        VirtualView.Create?.Invoke(MauiContext!)
        ?? throw new InvalidOperationException($"Set {nameof(PlatformViewHost)}.{nameof(PlatformViewHost.Create)} before the view is shown.");

    protected override void DisconnectHandler(PlatformView platformView)
    {
        VirtualView?.Release?.Invoke(platformView);
        base.DisconnectHandler(platformView);
    }
}
