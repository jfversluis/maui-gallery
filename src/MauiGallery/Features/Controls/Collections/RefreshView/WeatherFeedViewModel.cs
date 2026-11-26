using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IconFont.Maui.FluentIcons;

namespace MauiGallery.Features.Controls.Collections;

/// <summary>Current weather in a city, for the RefreshView samples.</summary>
public sealed record CityWeather(string City, int Temperature, string Condition, string Glyph)
{
    public string TemperatureText => $"{Temperature}°";
}

/// <summary>The view model behind the "Pull to refresh a list" RefreshView sample.</summary>
public sealed partial class WeatherFeedViewModel : ObservableObject
{
    private static readonly (string City, int Average)[] KnownCities =
    [
        ("Lisbon", 17), ("Oslo", 2), ("Tokyo", 12), ("Nairobi", 21), ("Lima", 20), ("Sydney", 24), ("Reykjavík", 1), ("Toronto", 4),
    ];

    private static readonly (string Condition, string Glyph)[] Conditions =
    [
        ("Sunny", FluentIconsRegular.WeatherSunnyHigh24),
        ("Partly cloudy", FluentIconsRegular.WeatherPartlyCloudyDay24),
        ("Cloudy", FluentIconsRegular.WeatherCloudy24),
        ("Rain", FluentIconsRegular.WeatherRain24),
        ("Thunderstorms", FluentIconsRegular.WeatherThunderstorm24),
    ];

    private readonly Random _random = new(42);

    public WeatherFeedViewModel() => Cities = CreateReadings();

    [ObservableProperty]
    public partial IReadOnlyList<CityWeather> Cities { get; private set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartRefreshCommand))]
    public partial bool IsRefreshing { get; set; }

    [ObservableProperty]
    public partial string LastUpdated { get; private set; } = "Pull down to refresh";

    public event EventHandler? Refreshed;

    // RefreshView runs this when people pull down, or when IsRefreshing becomes true.
    [RelayCommand]
    private async Task RefreshAsync()
    {
        // Stands in for a call to a weather service.
        await Task.Delay(1200);
        Cities = CreateReadings();
        LastUpdated = $"Updated at {DateTime.Now:T}";
        IsRefreshing = false;
        Refreshed?.Invoke(this, EventArgs.Empty);
    }

    // For a refresh button: desktop pointers can't pull to refresh.
    [RelayCommand(CanExecute = nameof(CanStartRefresh))]
    private void StartRefresh() => IsRefreshing = true;

    private bool CanStartRefresh() => !IsRefreshing;

    private CityWeather[] CreateReadings() =>
        [.. KnownCities.Select(c =>
        {
            var (condition, glyph) = Conditions[_random.Next(Conditions.Length)];
            return new CityWeather(c.City, c.Average + _random.Next(-3, 4), condition, glyph);
        })];
}
