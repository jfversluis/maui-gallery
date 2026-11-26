using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.Controls.Text;

/// <summary>The view model behind the "Filter a list as you type" SearchBar sample.</summary>
public sealed partial class SearchBarFilterViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Query { get; set; } = string.Empty;

    [ObservableProperty]
    public partial IReadOnlyList<string> Results { get; private set; } = Fruit.All;

    partial void OnQueryChanged(string value) =>
        Results = string.IsNullOrWhiteSpace(value)
            ? Fruit.All
            : [.. Fruit.All.Where(f => f.Contains(value.Trim(), StringComparison.OrdinalIgnoreCase))];
}
