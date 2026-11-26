using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Collections;

/// <summary>A to-do in the "Show an empty view" BindableLayout sample.</summary>
public sealed record TodoItem(string Title, string Due);

/// <summary>The view model behind the "Show an empty view" BindableLayout sample.</summary>
public sealed partial class TaskListViewModel : ObservableObject
{
    private static readonly TodoItem[] Today =
    [
        new("Book the airport transfer", "9:00"),
        new("Pack the camera charger", "12:30"),
        new("Check in for the flight", "18:00"),
    ];

    public ObservableCollection<TodoItem> Tasks { get; } = [.. Today];

    public event EventHandler<string>? TasksChanged;

    // Concurrent, so people can check off the next task while the previous one is still fading out.
    [RelayCommand(AllowConcurrentExecutions = true)]
    private async Task CompleteAsync(TodoItem task)
    {
        // Leave the check mark visible for a moment before the row goes away.
        await Task.Delay(400);
        Tasks.Remove(task);
        TasksChanged?.Invoke(this, $"Completed \"{task.Title}\" ({Tasks.Count} left)");
    }

    [RelayCommand]
    private void Restore()
    {
        Tasks.Clear();
        foreach (var task in Today)
        {
            Tasks.Add(task);
        }

        TasksChanged?.Invoke(this, $"Restored {Tasks.Count} tasks");
    }
}
