using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Fundamentals;

/// <summary>The view model behind the "Async commands" sample: a slow search that can be cancelled.</summary>
public sealed partial class CitySearchViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Result { get; set; } = "Search to see the forecast";

    // MAUI's compiled bindings can't see the generated SearchCancelCommand, so expose it through a declared property.
    public ICommand CancelCommand => SearchCancelCommand;

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task SearchAsync(string? city, CancellationToken cancellationToken)
    {
        city = string.IsNullOrWhiteSpace(city) ? "Amsterdam" : city.Trim();
        Result = $"Looking up {city}…";
        try
        {
            // Stands in for a slow web request.
            await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);
            Result = $"{city}: 21° and sunny";
        }
        catch (OperationCanceledException)
        {
            Result = "Search cancelled";
        }
    }
}
