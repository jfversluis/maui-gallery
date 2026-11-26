using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Pickers;

/// <summary>A city in the "Bind to a list of objects" Picker sample.</summary>
public sealed record City(string Name, string Country, int Population);

/// <summary>The view model behind the "Bind to a list of objects" Picker sample.</summary>
public sealed partial class PickerBindingViewModel : ObservableObject
{
    public IReadOnlyList<City> Cities { get; } =
    [
        new("Amsterdam", "the Netherlands", 931_000),
        new("Cairo", "Egypt", 10_100_000),
        new("Lima", "Peru", 10_000_000),
        new("Seattle", "the United States", 755_000),
        new("Sydney", "Australia", 5_450_000),
        new("Tokyo", "Japan", 14_100_000),
    ];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    public partial City? SelectedCity { get; set; }

    public string Summary => SelectedCity is { } city
        ? $"{city.Name} is in {city.Country} and has about {city.Population:N0} people."
        : "No city selected yet.";

    [RelayCommand]
    private void PickRandom()
    {
        var others = Cities.Where(c => c != SelectedCity).ToList();
        SelectedCity = others[Random.Shared.Next(others.Count)];
    }
}
