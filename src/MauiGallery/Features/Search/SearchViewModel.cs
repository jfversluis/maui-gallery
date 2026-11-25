using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Catalog;
using MauiGallery.Services;

namespace MauiGallery.Features.Search;

public sealed partial class SearchViewModel(GalleryCatalog catalog, INavigationService navigation) : ObservableObject
{
    private readonly IReadOnlyList<GalleryItem> _allItems = [.. catalog.Items.OrderBy(i => i.Title, StringComparer.CurrentCultureIgnoreCase)];

    [ObservableProperty]
    public partial string? Query { get; set; }

    public IReadOnlyList<GalleryItem> Results =>
        string.IsNullOrWhiteSpace(Query) ? _allItems : [.. catalog.Search.Search(Query).Select(r => r.Item)];

    public string ResultsTitle => string.IsNullOrWhiteSpace(Query) ? "Everything in the gallery" : "Results";

    partial void OnQueryChanged(string? value)
    {
        OnPropertyChanged(nameof(Results));
        OnPropertyChanged(nameof(ResultsTitle));
    }

    [RelayCommand]
    private Task OpenItemAsync(GalleryItem item) => navigation.GoToItemAsync(item.Id);
}
