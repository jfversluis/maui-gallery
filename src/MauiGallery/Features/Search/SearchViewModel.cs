using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Catalog;
using MauiGallery.Services;

namespace MauiGallery.Features.Search;

public sealed partial class SearchViewModel : ObservableObject
{
    private readonly GalleryCatalog _catalog;
    private readonly INavigationService _navigation;
    private readonly IReadOnlyList<GalleryItem> _allItems;

    public SearchViewModel(GalleryCatalog catalog, INavigationService navigation)
    {
        ArgumentNullException.ThrowIfNull(catalog);

        _catalog = catalog;
        _navigation = navigation;
        _allItems = [.. catalog.Items.OrderBy(i => i.Title, StringComparer.CurrentCultureIgnoreCase)];
        Results = _allItems;
    }

    [ObservableProperty]
    public partial string? Query { get; set; }

    [ObservableProperty]
    public partial IReadOnlyList<GalleryItem> Results { get; private set; }

    public string ResultsTitle => string.IsNullOrWhiteSpace(Query) ? "Everything in the gallery" : "Results";

    partial void OnQueryChanged(string? value)
    {
        Results = string.IsNullOrWhiteSpace(value) ? _allItems : [.. _catalog.Search.Search(value).Select(r => r.Item)];
        OnPropertyChanged(nameof(ResultsTitle));
    }

    [RelayCommand]
    private Task OpenItemAsync(GalleryItem item) => _navigation.GoToItemAsync(item.Id);
}
