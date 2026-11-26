using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Collections;
using MauiGallery.Features.Catalog;
using MauiGallery.Services;

namespace MauiGallery.Features.Chrome;

public sealed partial class SidebarViewModel : ObservableObject
{
    private const int MaxSearchResults = 12;
    private const string ExpandedGroupsKey = "sidebar-expanded-groups";

    private readonly GalleryCatalog _catalog;
    private readonly INavigationService _navigation;
    private readonly IPreferences _preferences;
    private readonly List<SidebarEntry> _allEntries = [];

    // Groups the user opened with the chevron. The current page's group opens on its own and isn't remembered.
    private readonly HashSet<string> _userExpandedGroups;

    public SidebarViewModel(GalleryCatalog catalog, INavigationService navigation, IPreferences preferences)
    {
        _catalog = catalog;
        _navigation = navigation;
        _preferences = preferences;
        _userExpandedGroups = [.. IdListSerializer.Deserialize(preferences.Get(ExpandedGroupsKey, string.Empty))];

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

        foreach (var group in Groups)
        {
            group.IsExpanded = _userExpandedGroups.Contains(group.Category.Id);
        }
    }

    /// <summary>Raised when the highlighted entry changes, so the view can scroll it into view.</summary>
    public event EventHandler<SidebarEntry>? SelectionChanged;

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
        Select(_allEntries.FirstOrDefault(e => e.ItemId == lastSegment)
            ?? _allEntries.FirstOrDefault(e => e.ItemId is null && path.EndsWith(e.Route.TrimStart('/'), StringComparison.Ordinal)));
    }

    private void Select(SidebarEntry? selected)
    {
        foreach (var entry in _allEntries)
        {
            entry.IsSelected = ReferenceEquals(entry, selected);
        }

        if (selected is null)
        {
            return;
        }

        // Only the current page's group and the ones the user opened stay expanded.
        foreach (var group in Groups)
        {
            group.IsExpanded = _userExpandedGroups.Contains(group.Category.Id) || group.Entries.Contains(selected);
        }

        SelectionChanged?.Invoke(this, selected);
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
        // Highlight the entry right away, while the page is being built; the Shell confirms it once it has navigated.
        Select(entry);
        await _navigation.GoToAsync(entry.Route);
        Query = null;
    }

    [RelayCommand]
    private Task OpenFirstResultAsync() =>
        SearchResults.Count > 0 ? NavigateAsync(SearchResults[0]) : Task.CompletedTask;

    [RelayCommand]
    private void ToggleGroup(SidebarGroup group)
    {
        group.IsExpanded = !group.IsExpanded;
        if (group.IsExpanded)
        {
            _userExpandedGroups.Add(group.Category.Id);
        }
        else
        {
            _userExpandedGroups.Remove(group.Category.Id);
        }

        _preferences.Set(ExpandedGroupsKey, IdListSerializer.Serialize(_userExpandedGroups));
    }
}
