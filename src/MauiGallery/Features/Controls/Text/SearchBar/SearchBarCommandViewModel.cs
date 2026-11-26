using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Text;

/// <summary>The view model behind the "Run a search command" SearchBar sample.</summary>
public sealed partial class SearchBarCommandViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Status { get; set; } = "Results appear here";

    [RelayCommand]
    private async Task SearchAsync(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            Status = "Type something to search for";
            return;
        }

        Status = "Searching…";

        // Stands in for a call to a search service.
        await Task.Delay(TimeSpan.FromSeconds(1));

        var count = Fruit.All.Count(f => f.Contains(query.Trim(), StringComparison.OrdinalIgnoreCase));
        Status = count switch
        {
            0 => $"No results for “{query}”",
            1 => $"1 result for “{query}”",
            _ => $"{count} results for “{query}”",
        };
    }
}
