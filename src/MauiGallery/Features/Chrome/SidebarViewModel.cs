using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IconFont.Maui.FluentIcons;
using MauiGallery.Features.Catalog;
using MauiGallery.Services;

namespace MauiGallery.Features.Chrome;

public sealed partial class SidebarViewModel : ObservableObject
{
    private const int MaxSearchResults = 12;

    private readonly GalleryCatalog _catalog;
    private readonly INavigationService _navigation;
    private readonly List<SidebarEntry> _allEntries = [];

    public SidebarViewModel(GalleryCatalog catalog, INavigationService navigation)
    {
        _catalog = catalog;
        _navigation = navigation;

        TopEntries =
        [
            new("Home", FluentIconsRegular.Home24, Routes.Home),
            new("All controls", FluentIconsRegular.Apps24, Routes.AllControls),
        ];
        FooterEntries = [new("Settings", FluentIconsRegular.Settings24, Routes.Settings)];
        Groups =
        [
            .. catalog.PopulatedCategories.Select(category => new SidebarGroup(
                category,
                [.. catalog.InCategory(category.Id).Select(item => new SidebarEntry(item.Title, item.IconGlyph, $"{Routes.AllControls}/{item.Id}", item.Id))])),
        ];

        _allEntries.AddRange(TopEntries);
        _allEntries.AddRange(FooterEntries);
        _allEntries.AddRange(Groups.SelectMany(g => g.Entries));
    }

    public IReadOnlyList<SidebarEntry> TopEntries { get; }

    public IReadOnlyList<SidebarGroup> Groups { get; }

    public IReadOnlyList<SidebarEntry> FooterEntries { get; }

    public ObservableCollection<SidebarEntry> SearchResults { get; } = [];

    [ObservableProperty]
    public partial string? Query { get; set; }

    [ObservableProperty]
    public partial bool IsSearching { get; set; }

    [ObservableProperty]
    public partial bool HasNoResults { get; set; }

    /// <summary>Highlights the entry that matches the current Shell location.</summary>
    public void UpdateSelection(string location)
    {
        var path = location.Split('?')[0].TrimEnd('/');
        var lastSegment = path[(path.LastIndexOf('/') + 1)..];
        var selected = _allEntries.FirstOrDefault(e => e.ItemId == lastSegment)
            ?? _allEntries.FirstOrDefault(e => e.ItemId is null && path.EndsWith(e.Route.TrimStart('/'), StringComparison.Ordinal));

        foreach (var entry in _allEntries)
        {
            entry.IsSelected = ReferenceEquals(entry, selected);
        }
    }

    partial void OnQueryChanged(string? value)
    {
        SearchResults.Clear();
        IsSearching = !string.IsNullOrWhiteSpace(value);
        foreach (var result in _catalog.Search.Search(value, MaxSearchResults))
        {
            SearchResults.Add(_allEntries.First(e => e.ItemId == result.Item.Id));
        }

        HasNoResults = IsSearching && SearchResults.Count == 0;
    }

    [RelayCommand]
    private async Task NavigateAsync(SidebarEntry entry)
    {
        await _navigation.GoToAsync(entry.Route);
        Query = null;
    }

    [RelayCommand]
    private Task OpenFirstResultAsync() =>
        SearchResults.Count > 0 ? NavigateAsync(SearchResults[0]) : Task.CompletedTask;

    [RelayCommand]
    private static void ToggleGroup(SidebarGroup group) => group.IsExpanded = !group.IsExpanded;
}
