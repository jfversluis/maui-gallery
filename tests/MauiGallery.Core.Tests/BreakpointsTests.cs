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
}
