using CommunityToolkit.Mvvm.Input;
using MauiGallery.Core.Catalog;
using MauiGallery.Services;

namespace MauiGallery.Features.Catalog;

public sealed partial class AllControlsViewModel(GalleryCatalog catalog, INavigationService navigation)
{
    public IReadOnlyList<CategorySection> Sections { get; } =
        [.. catalog.PopulatedCategories.Select(c => new CategorySection(c, [.. catalog.InCategory(c.Id)]))];

    public int ItemCount => catalog.Items.Count;

    [RelayCommand]
    private Task OpenItemAsync(GalleryItem item) => navigation.GoToItemAsync(item.Id);
}
