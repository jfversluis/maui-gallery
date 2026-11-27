namespace MauiGallery.Core.Layout;

public static class Breakpoints
{
    public const double Medium = 640;

    public const double Expanded = 1100;

    /// <summary>Below this height a window is a phone in landscape: too short for a sidebar.</summary>
    public const double CompactHeight = 480;

    public static SizeClass Classify(double width) => width switch
    {
        >= Expanded => SizeClass.Expanded,
        >= Medium => SizeClass.Medium,
        _ => SizeClass.Compact,
    };

    /// <summary>
    /// Chooses the navigation for a window from its size, not the device type, so a foldable that opens up or an iPad
    /// app in a narrow split view gets the navigation that fits. Small touch windows use bottom tabs; with a mouse and
    /// keyboard, a narrow window keeps the sidebar as a flyout, as desktop apps do.
    /// </summary>
    /// <param name="width">The window width.</param>
    /// <param name="height">The window height, or 0 when it isn't known yet.</param>
    /// <param name="isDesktop">Whether the device has a mouse and keyboard (Mac and Windows).</param>
    public static NavigationChrome ChromeFor(double width, double height, bool isDesktop) => Classify(width) switch
    {
        SizeClass.Expanded when height is 0 or >= CompactHeight => NavigationChrome.DockedSidebar,
        SizeClass.Compact when isDesktop => NavigationChrome.FlyoutSidebar,
        SizeClass.Compact => NavigationChrome.Tabs,
        _ when !isDesktop && height is > 0 and < CompactHeight => NavigationChrome.Tabs,
        _ => NavigationChrome.FlyoutSidebar,
    };
}
