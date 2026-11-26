using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Fundamentals;

/// <summary>The view model behind the "Run a command on an event" sample.</summary>
public sealed partial class UsernameViewModel : ObservableObject
{
    private static readonly string[] TakenNames = ["admin", "maui", "dotnet"];

    [ObservableProperty]
    public partial string Status { get; set; } = "Pick a username, then move to the next field.";

    [RelayCommand]
    private async Task CheckAvailabilityAsync(string? username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            Status = "Pick a username, then move to the next field.";
            return;
        }

        username = username.Trim();
        Status = $"Checking \"{username}\"…";

        // Stands in for a call to a web service.
        await Task.Delay(600);
        Status = TakenNames.Contains(username, StringComparer.OrdinalIgnoreCase)
            ? $"\"{username}\" is taken. Try another one."
            : $"\"{username}\" is available.";
    }
}
