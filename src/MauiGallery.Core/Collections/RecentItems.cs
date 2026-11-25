namespace MauiGallery.Core.Collections;

/// <summary>A most-recent-first list of identifiers without duplicates and with a fixed capacity.</summary>
public sealed class RecentItems
{
    private readonly List<string> _ids;

    public RecentItems(int capacity, IEnumerable<string>? initial = null)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(capacity, 1);

        Capacity = capacity;
        _ids = [.. (initial ?? []).Where(id => !string.IsNullOrWhiteSpace(id)).Distinct(StringComparer.Ordinal).Take(capacity)];
    }

    public int Capacity { get; }

    public IReadOnlyList<string> Ids => _ids;

    /// <summary>Moves <paramref name="id"/> to the front. Returns <see langword="true"/> if the list changed.</summary>
    public bool Add(string id)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(id);

        if (_ids.Count > 0 && _ids[0] == id)
        {
            return false;
        }

        _ids.Remove(id);
        _ids.Insert(0, id);
        if (_ids.Count > Capacity)
        {
            _ids.RemoveAt(_ids.Count - 1);
        }

        return true;
    }

    public void Clear() => _ids.Clear();
}
