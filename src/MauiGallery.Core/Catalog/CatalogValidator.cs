using System.Text.RegularExpressions;

namespace MauiGallery.Core.Catalog;

/// <summary>Checks a catalog for mistakes that would otherwise only surface at runtime.</summary>
public static partial class CatalogValidator
{
    /// <summary>Returns a human-readable description of every problem found. An empty list means the catalog is valid.</summary>
    public static IReadOnlyList<string> Validate(IReadOnlyCollection<GalleryCategory> categories, IReadOnlyCollection<GalleryItem> items)
    {
        ArgumentNullException.ThrowIfNull(categories);
        ArgumentNullException.ThrowIfNull(items);

        var errors = new List<string>();
        var categoryIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var category in categories)
        {
            if (!categoryIds.Add(category.Id))
            {
                errors.Add($"Duplicate category id '{category.Id}'.");
            }
        }

        var itemIds = new HashSet<string>(StringComparer.Ordinal);
        foreach (var item in items)
        {
            if (!itemIds.Add(item.Id))
            {
                errors.Add($"Duplicate item id '{item.Id}'.");
            }

            if (!IdPattern().IsMatch(item.Id))
            {
                errors.Add($"Item id '{item.Id}' must be lowercase kebab-case so it can be used as a route.");
            }

            if (!categoryIds.Contains(item.CategoryId))
            {
                errors.Add($"Item '{item.Id}' references unknown category '{item.CategoryId}'.");
            }

            if (item.Platforms == GalleryPlatforms.None)
            {
                errors.Add($"Item '{item.Id}' must run on at least one platform.");
            }

            if (string.IsNullOrWhiteSpace(item.Title) || string.IsNullOrWhiteSpace(item.Description))
            {
                errors.Add($"Item '{item.Id}' needs a title and a description.");
            }
        }

        foreach (var item in items)
        {
            foreach (var relatedId in item.RelatedIds)
            {
                if (relatedId == item.Id)
                {
                    errors.Add($"Item '{item.Id}' lists itself as related.");
                }
                else if (!itemIds.Contains(relatedId))
                {
                    errors.Add($"Item '{item.Id}' references unknown related item '{relatedId}'.");
                }
            }
        }

        return errors;
    }

    [GeneratedRegex("^[a-z0-9]+(-[a-z0-9]+)*$")]
    private static partial Regex IdPattern();
}
