using System.Collections.Frozen;
using MauiGallery.Core.Catalog;
using MauiGallery.Core.Search;

namespace MauiGallery.Features.Catalog;

/// <summary>Every page in the gallery, grouped into categories. The single source of truth for navigation and search.</summary>
public sealed partial class GalleryCatalog
{
    private readonly FrozenDictionary<string, GalleryItem> _itemsById;
    private readonly FrozenDictionary<Type, GalleryItem> _itemsByPage;
    private readonly FrozenDictionary<string, GalleryCategory> _categoriesById;

    public GalleryCatalog()
    {
        Categories = CreateCategories();
        Items = CreateItems();

        var errors = CatalogValidator.Validate(Categories, Items);
        if (errors.Count > 0)
        {
            throw new InvalidOperationException($"The gallery catalog is invalid:{Environment.NewLine}{string.Join(Environment.NewLine, errors)}");
        }

        _itemsById = Items.ToFrozenDictionary(i => i.Id, StringComparer.Ordinal);
        _itemsByPage = Items.ToFrozenDictionary(i => i.PageType);
        _categoriesById = Categories.ToFrozenDictionary(c => c.Id, StringComparer.Ordinal);
        Search = new CatalogSearch(Items, Categories);
    }

    public IReadOnlyList<GalleryCategory> Categories { get; }

    public IReadOnlyList<GalleryItem> Items { get; }

    public CatalogSearch Search { get; }

    /// <summary>Categories that contain at least one item, in display order.</summary>
    public IEnumerable<GalleryCategory> PopulatedCategories => Categories.Where(c => Items.Any(i => i.CategoryId == c.Id));

    public IEnumerable<GalleryItem> Featured => Items.Where(i => i.IsFeatured);

    public GalleryItem? Find(string id) => _itemsById.GetValueOrDefault(id);

    public GalleryItem? FindByPage(Type pageType) => _itemsByPage.GetValueOrDefault(pageType);

    public GalleryCategory CategoryOf(GalleryItem item) => _categoriesById[item.CategoryId];

    public IEnumerable<GalleryItem> InCategory(string categoryId) => Items.Where(i => i.CategoryId == categoryId);
}
