using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiGallery.Core.Catalog;

namespace MauiGallery.Features.Samples;

/// <summary>Header, favorite, link and related-item state shared by every sample page.</summary>
public sealed partial class SamplePageViewModel : ObservableObject
{
    private readonly SamplePageServices _services;

    public SamplePageViewModel(GalleryItem item, SamplePageServices services)
    {
        ArgumentNullException.ThrowIfNull(services);

        _services = services;
        Item = item;
        Category = services.Catalog.CategoryOf(item);
        NativeMappings = [.. item.NativeMappings.Select(NativeMappingDisplay.From)];
        Related = [.. item.RelatedIds.Select(services.Catalog.Find).OfType<GalleryItem>()];
        IsFavorite = services.Favorites.IsFavorite(item.Id);
    }

    public GalleryItem Item { get; }

    public GalleryCategory Category { get; }

    public IReadOnlyList<NativeMappingDisplay> NativeMappings { get; }

    public bool HasNativeMappings => NativeMappings.Count > 0;

    public IReadOnlyList<GalleryItem> Related { get; }

    public bool HasRelated => Related.Count > 0;

    /// <summary>The link that opens this page directly, for example <c>mauigallery://item/button</c>.</summary>
    public string DeepLink => $"{DeepLinks.Scheme}://item/{Item.Id}";

    [ObservableProperty]
    public partial bool IsFavorite { get; set; }

    [ObservableProperty]
    public partial bool LinkCopied { get; set; }

    public void OnAppearing() => _services.Recents.Add(Item.Id);

    [RelayCommand]
    private void ToggleFavorite()
    {
        IsFavorite = _services.Favorites.Toggle(Item.Id);
        SemanticScreenReader.Announce(IsFavorite ? $"{Item.Title} added to favorites" : $"{Item.Title} removed from favorites");
    }

    [RelayCommand]
    private Task OpenLinkAsync(Uri? uri) =>
        uri is null ? Task.CompletedTask : _services.Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);

    [RelayCommand]
    private async Task CopyLinkAsync()
    {
        await _services.Clipboard.SetTextAsync(DeepLink);
        LinkCopied = true;
        SemanticScreenReader.Announce("Link copied");
        await Task.Delay(TimeSpan.FromSeconds(1.5));
        LinkCopied = false;
    }

    [RelayCommand]
    private Task OpenItemAsync(GalleryItem item) => _services.Navigation.GoToItemAsync(item.Id);
}
