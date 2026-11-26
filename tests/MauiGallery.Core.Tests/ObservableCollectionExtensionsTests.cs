using System.Collections.ObjectModel;
using System.Collections.Specialized;
using MauiGallery.Core.Collections;

namespace MauiGallery.Core.Tests;

public sealed class ObservableCollectionExtensionsTests
{
    [Theory]
    [InlineData("abcd", "xabc", "Add,Remove")]
    [InlineData("abcd", "cabd", "Move")]
    [InlineData("abcd", "abcd", "")]
    [InlineData("", "ab", "Add,Add")]
    [InlineData("abc", "", "Remove,Remove,Remove")]
    [InlineData("abc", "cxa", "Move,Add,Remove")]
    public void SyncWithMakesTheMinimalChanges(string before, string after, string changes)
    {
        var target = new ObservableCollection<char>(before);
        var actions = new List<NotifyCollectionChangedAction>();
        target.CollectionChanged += (_, e) => actions.Add(e.Action);

        target.SyncWith(after.ToList());

        Assert.Equal(after, string.Concat(target));
        Assert.Equal(changes, string.Join(',', actions));
    }
}
