using MauiGallery.Core.Layout;

namespace MauiGallery.Core.Tests;

public sealed class BreakpointsTests
{
    [Theory]
    [InlineData(0, SizeClass.Compact)]
    [InlineData(639.9, SizeClass.Compact)]
    [InlineData(640, SizeClass.Medium)]
    [InlineData(1099, SizeClass.Medium)]
    [InlineData(1100, SizeClass.Expanded)]
    public void Classifies(double width, SizeClass expected) => Assert.Equal(expected, Breakpoints.Classify(width));

    [Theory]
    [InlineData(402, 874, false, NavigationChrome.Tabs)]            // iPhone, portrait
    [InlineData(874, 402, false, NavigationChrome.Tabs)]            // iPhone, landscape: too short for a sidebar
    [InlineData(841, 701, false, NavigationChrome.FlyoutSidebar)]   // Foldable, open
    [InlineData(320, 1024, false, NavigationChrome.Tabs)]           // iPad, narrow split view
    [InlineData(1366, 1024, false, NavigationChrome.DockedSidebar)] // iPad, landscape
    [InlineData(1300, 430, false, NavigationChrome.Tabs)]           // Wide but short touch window
    [InlineData(520, 860, true, NavigationChrome.FlyoutSidebar)]    // Narrow desktop window
    [InlineData(1440, 900, true, NavigationChrome.DockedSidebar)]   // Desktop
    [InlineData(1440, 0, true, NavigationChrome.DockedSidebar)]     // Height not known yet
    public void ChoosesNavigation(double width, double height, bool isDesktop, NavigationChrome expected) =>
        Assert.Equal(expected, Breakpoints.ChromeFor(width, height, isDesktop));
}
