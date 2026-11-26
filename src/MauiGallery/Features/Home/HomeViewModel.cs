using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Catalog;
using MauiGallery.Services;

namespace MauiGallery.Features.Home;

public sealed partial class HomeViewModel : ObservableObject,
    IRecipient<FavoritesChangedMessage>,
    IRecipient<RecentsChangedMessage>
{
    private readonly GalleryCatalog _catalog;
    private readonly IFavoritesService _favorites;
    private readonly IRecentsService _recents;
    private readonly INavigationService _navigation;
    private readonly IBrowser _browser;

    public HomeViewModel(
        GalleryCatalog catalog,
        IFavoritesService favorites,
        IRecentsService recents,
        INavigationService navigation,
        IBrowser browser,
        IMessenger messenger)
    {
        ArgumentNullException.ThrowIfNull(messenger);

        _catalog = catalog;
        _favorites = favorites;
        _recents = recents;
        _navigation = navigation;
        _browser = browser;

        Featured = [.. catalog.Featured];
        HasFeatured = Featured.Count > 0;
        Categories = [.. catalog.PopulatedCategories.Select(c => new CategoryTile(c, catalog.InCategory(c.Id).Count()))];
        RefreshFavorites();
        RefreshRecents();
        messenger.Register<FavoritesChangedMessage>(this);
        messenger.Register<RecentsChangedMessage>(this);
    }

    public IReadOnlyList<GetStartedLink> GetStarted { get; } =
    [
        new("Install .NET MAUI", "Set up Visual Studio, VS Code or the CLI.", FluentIconsRegular.ArrowDownload24, new("https://learn.microsoft.com/dotnet/maui/get-started/installation")),
        new("Documentation", "Guides and references for every control and API.", FluentIconsRegular.BookOpen24, new("https://learn.microsoft.com/dotnet/maui/")),
        new("Learn path", "Free, hands-on modules from first app to publishing.", FluentIconsRegular.HatGraduation24, new("https://learn.microsoft.com/training/paths/build-apps-with-dotnet-maui/")),
        new("What's new", "The latest in .NET MAUI.", FluentIconsRegular.Sparkle24, new("https://learn.microsoft.com/dotnet/maui/whats-new/")),
    ];

    /// <summary>A short, curated tour: one item per task, labeled with it. Always complete, even if visited.</summary>
    public IReadOnlyList<GalleryItem> Featured { get; }

    public IReadOnlyList<CategoryTile> Categories { get; }

    public ObservableCollection<GalleryItem> Recents { get; } = [];

    public ObservableCollection<GalleryItem> Favorites { get; } = [];

    [ObservableProperty]
    public partial bool HasRecents { get; set; }

    [ObservableProperty]
    public partial bool HasFavorites { get; set; }

    public bool HasFeatured { get; }

    public void Receive(FavoritesChangedMessage message) => RefreshFavorites();

    public void Receive(RecentsChangedMessage message) => RefreshRecents();

    [RelayCommand]
    private Task OpenItemAsync(GalleryItem item) => _navigation.GoToItemAsync(item.Id);

    [RelayCommand]
    private Task OpenCategoryAsync(CategoryTile tile) => _navigation.GoToAsync($"{Routes.Category}?id={Uri.EscapeDataString(tile.Category.Id)}");

    [RelayCommand]
    private Task BrowseControlsAsync() => _navigation.GoToAsync(Routes.AllControls);

    [RelayCommand]
    private async Task OpenLinkAsync(GetStartedLink link) => await _browser.OpenAsync(link.Url, BrowserLaunchMode.SystemPreferred);

    private void RefreshFavorites()
    {
        Replace(Favorites, _favorites.Ids);
        HasFavorites = Favorites.Count > 0;
    }

    private void RefreshRecents()
    {
        Replace(Recents, _recents.Ids.Take(4));
        HasRecents = Recents.Count > 0;
    }

    private void Replace(ObservableCollection<GalleryItem> target, IEnumerable<string> ids)
    {
        target.Clear();
        foreach (var item in ids.Select(_catalog.Find).OfType<GalleryItem>())
        {
            target.Add(item);
        }
    }
}
