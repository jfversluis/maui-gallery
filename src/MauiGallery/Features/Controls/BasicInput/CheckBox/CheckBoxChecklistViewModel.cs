using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.Controls.BasicInput;

/// <summary>The view model behind the "Bind a list of checkboxes" CheckBox sample.</summary>
public sealed partial class CheckBoxChecklistViewModel : ObservableObject
{
    public CheckBoxChecklistViewModel()
    {
        Tasks =
        [
            new("Install the .NET MAUI workload", "sample-list-install") { IsDone = true },
            new("Create a new project", "sample-list-create"),
            new("Run it on a device", "sample-list-run"),
        ];

        foreach (var task in Tasks)
        {
            task.PropertyChanged += (_, _) => OnPropertyChanged(nameof(Summary));
        }
    }

    public IReadOnlyList<ChecklistTask> Tasks { get; }

    public string Summary => Tasks.Count(t => t.IsDone) switch
    {
        var done when done == Tasks.Count => "All done!",
        var done => $"{done} of {Tasks.Count} done",
    };
}

/// <summary>One item in the CheckBox checklist sample.</summary>
public sealed partial class ChecklistTask(string title, string automationId) : ObservableObject
{
    public string Title { get; } = title;

    public string AutomationId { get; } = automationId;

    [ObservableProperty]
    public partial bool IsDone { get; set; }
}
