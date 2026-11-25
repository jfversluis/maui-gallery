using MauiGallery.Core.Collections;

namespace MauiGallery.Core.Tests;

public sealed class RecentItemsTests
{
    [Fact]
    public void AddMovesExistingItemToFront()
    {
        var recents = new RecentItems(3, ["a", "b", "c"]);

        Assert.True(recents.Add("c"));
        Assert.Equal(["c", "a", "b"], recents.Ids);
    }

    [Fact]
    public void AddingCurrentFrontItemIsNoChange() => Assert.False(new RecentItems(3, ["a"]).Add("a"));

    [Fact]
    public void CapacityIsEnforced()
    {
        var recents = new RecentItems(2, ["a", "b"]);

        recents.Add("c");

        Assert.Equal(["c", "a"], recents.Ids);
    }

    [Fact]
    public void InitialValuesAreDeduplicatedAndTrimmedToCapacity() =>
        Assert.Equal(["a", "b"], new RecentItems(2, ["a", "a", " ", "b", "c"]).Ids);

    [Fact]
    public void RoundTripsThroughSerializer()
    {
        var text = IdListSerializer.Serialize(["button", "label"]);

        Assert.Equal(["button", "label"], IdListSerializer.Deserialize(text));
        Assert.Empty(IdListSerializer.Deserialize(null));
    }
}
