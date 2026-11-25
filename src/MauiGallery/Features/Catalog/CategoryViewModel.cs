using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiGallery.Core.Catalog;
using MauiGallery.Services;

namespace MauiGallery.Features.Catalog;

/// <summary>Lists the items in one category. Receives the category through the <c>id</c> query parameter.</summary>
public sealed partial class CategoryViewModel(GalleryCatalog catalog, INavigationService navigation) : ObservableObject, IQueryAttributable
{
    [ObservableProperty]
    public partial GalleryCategory? Category { get; set; }

    [ObservableProperty]
    public partial IReadOnlyList<GalleryItem> Items { get; set; } = [];

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("id", out var value) && value is string id)
        {
            id = Uri.UnescapeDataString(id);
            Category = catalog.Categories.FirstOrDefault(c => c.Id == id);
            Items = [.. catalog.InCategory(id)];
        }
    }

    [RelayCommand]
    private Task OpenItemAsync(GalleryItem item) => navigation.GoToItemAsync(item.Id);
}
