using MauiGallery.Core.Navigation;

namespace MauiGallery.Core.Tests;

public sealed class DeepLinkTests
{
    [Theory]
    [InlineData("mauigallery://item/button", "button")]
    [InlineData("mauigallery://control/date-picker", "date-picker")]
    [InlineData("MAUIGALLERY://Item/Button/", "button")]
    public void ParsesItemLinks(string link, string expected) => Assert.Equal(expected, DeepLink.TryGetItemId(new Uri(link)));

    [Theory]
    [InlineData("https://item/button")]
    [InlineData("mauigallery://settings/theme")]
    [InlineData("mauigallery://item/")]
    [InlineData("mauigallery://item/a/b")]
    public void RejectsOtherLinks(string link) => Assert.Null(DeepLink.TryGetItemId(new Uri(link)));

    [Fact]
    public void RejectsNull() => Assert.Null(DeepLink.TryGetItemId(null));

    [Fact]
    public void RoundTrips() => Assert.Equal("button", DeepLink.TryGetItemId(DeepLink.ForItem("button")));
}
