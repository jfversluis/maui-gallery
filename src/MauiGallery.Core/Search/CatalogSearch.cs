using System.Collections.Frozen;
using System.Globalization;
using MauiGallery.Core.Catalog;

namespace MauiGallery.Core.Search;

/// <summary>
/// Ranks gallery items against a free-text query. Every term in the query must match the item's title,
/// keywords, category or description. Matching ignores case and diacritics.
/// </summary>
public sealed class CatalogSearch
{
    private const CompareOptions MatchOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace;

    private static readonly CompareInfo Compare = CultureInfo.InvariantCulture.CompareInfo;

    private readonly IReadOnlyList<GalleryItem> _items;
    private readonly FrozenDictionary<string, string> _categoryTitles;

    public CatalogSearch(IReadOnlyList<GalleryItem> items, IReadOnlyCollection<GalleryCategory> categories)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(categories);

        _items = items;
        _categoryTitles = categories.ToFrozenDictionary(c => c.Id, c => c.Title, StringComparer.Ordinal);
    }

    /// <summary>Returns matching items, best match first. An empty or whitespace query returns no results.</summary>
    public IReadOnlyList<SearchResult> Search(string? query, int maxResults = int.MaxValue)
    {
        var terms = (query ?? string.Empty).Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (terms.Length == 0)
        {
            return [];
        }

        var results = new List<SearchResult>();
        foreach (var item in _items)
        {
            var score = 0;
            foreach (var term in terms)
            {
                var termScore = ScoreTerm(item, term);
                if (termScore == 0)
                {
                    score = 0;
                    break;
                }

                score += termScore;
            }

            if (score > 0)
            {
                results.Add(new SearchResult(item, score));
            }
        }

        return [.. results
            .OrderByDescending(r => r.Score)
            .ThenBy(r => r.Item.Title, StringComparer.CurrentCultureIgnoreCase)
            .Take(maxResults)];
    }

    private int ScoreTerm(GalleryItem item, string term)
    {
        if (Compare.Compare(item.Title, term, MatchOptions) == 0)
        {
            return 100;
        }

        if (Compare.IsPrefix(item.Title, term, MatchOptions))
        {
            return 80;
        }

        if (StartsAnyWord(item.Title, term))
        {
            return 60;
        }

        if (item.Keywords.Any(k => Compare.IsPrefix(k, term, MatchOptions)))
        {
            return 50;
        }

        if (Contains(item.Title, term))
        {
            return 40;
        }

        if (_categoryTitles.TryGetValue(item.CategoryId, out var category) && StartsAnyWord(category, term))
        {
            return 20;
        }

        return Contains(item.Description, term) ? 10 : 0;
    }

    private static bool Contains(string source, string term) => Compare.IndexOf(source, term, MatchOptions) >= 0;

    private static bool StartsAnyWord(string source, string term)
    {
        foreach (var word in source.Split([' ', '-', '/', '&'], StringSplitOptions.RemoveEmptyEntries))
        {
            if (Compare.IsPrefix(word, term, MatchOptions))
            {
                return true;
            }
        }

        return false;
    }
}
