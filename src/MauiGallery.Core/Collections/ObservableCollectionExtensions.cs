using System.Collections.ObjectModel;

namespace MauiGallery.Core.Collections;

public static class ObservableCollectionExtensions
{
    /// <summary>
    /// Makes <paramref name="target"/> contain <paramref name="items"/>, in order, with as few changes as possible:
    /// items that stay are moved rather than removed and added again, so a bound layout rebuilds only what changed.
    /// </summary>
    public static void SyncWith<T>(this ObservableCollection<T> target, IReadOnlyList<T> items)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(items);

        var comparer = EqualityComparer<T>.Default;
        for (var i = 0; i < items.Count; i++)
        {
            if (i < target.Count && comparer.Equals(target[i], items[i]))
            {
                continue;
            }

            var existing = IndexOf(target, items[i], i + 1, comparer);
            if (existing >= 0)
            {
                target.Move(existing, i);
            }
            else
            {
                target.Insert(i, items[i]);
            }
        }

        while (target.Count > items.Count)
        {
            target.RemoveAt(target.Count - 1);
        }
    }

    private static int IndexOf<T>(ObservableCollection<T> list, T item, int start, EqualityComparer<T> comparer)
    {
        for (var i = start; i < list.Count; i++)
        {
            if (comparer.Equals(list[i], item))
            {
                return i;
            }
        }

        return -1;
    }
}
