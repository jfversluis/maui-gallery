using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.StatusAndInfo;

/// <summary>The view model behind the "Show a busy state while loading" ActivityIndicator sample.</summary>
public sealed partial class ActivityIndicatorForecastViewModel : ObservableObject
{
    private static readonly string[] Forecasts = ["Sunny, 24°", "Cloudy, 18°", "Light rain, 15°", "Windy, 20°"];

    private int _refreshCount;

    [ObservableProperty]
    public partial string Status { get; set; } = "Tap refresh to load the forecast";

    [RelayCommand]
    private async Task RefreshAsync()
    {
        Status = "Loading…";

        // Stands in for a network call
        await Task.Delay(TimeSpan.FromSeconds(2));

        Status = Forecasts[_refreshCount++ % Forecasts.Length];
    }
}
