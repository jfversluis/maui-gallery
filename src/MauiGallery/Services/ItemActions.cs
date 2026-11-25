using CommunityToolkit.Mvvm.Input;
using MauiGallery.Core.Catalog;
using MauiGallery.Core.Navigation;

namespace MauiGallery.Services;

/// <summary>Actions offered in the context menu of every gallery item card.</summary>
public sealed partial class ItemActions(
    INavigationService navigation,
    IWindowService windows,
    IFavoritesService favorites,
    IClipboard clipboard)
{
    public bool CanOpenWindows => windows.CanOpenWindows;

    public bool IsFavorite(GalleryItem item) => favorites.IsFavorite(item.Id);

    [RelayCommand]
    private Task OpenAsync(GalleryItem item) => navigation.GoToItemAsync(item.Id);

    [RelayCommand]
    private void OpenInNewWindow(GalleryItem item) => windows.OpenItem(item.Id);

    [RelayCommand]
    private void ToggleFavorite(GalleryItem item) => favorites.Toggle(item.Id);

    [RelayCommand]
    private Task CopyLinkAsync(GalleryItem item) => clipboard.SetTextAsync(DeepLink.ForItem(item.Id).ToString());
}
