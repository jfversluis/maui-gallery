using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.Controls.Collections;

/// <summary>The view model behind the "Header, footer and an empty view" CollectionView sample.</summary>
public sealed partial class PlaceSearchViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string SearchText { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    public partial IReadOnlyList<Place> Results { get; private set; } = PlaceData.All;

    public string Summary => Results.Count == 1 ? "1 place" : $"{Results.Count} places";

    partial void OnSearchTextChanged(string value) =>
        Results = [.. PlaceData.All.Where(p =>
            p.Name.Contains(value.Trim(), StringComparison.OrdinalIgnoreCase) ||
            p.Country.Contains(value.Trim(), StringComparison.OrdinalIgnoreCase))];
}
