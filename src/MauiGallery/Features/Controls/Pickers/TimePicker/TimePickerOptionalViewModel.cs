using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Pickers;

/// <summary>The view model behind the "Start empty and open from code" TimePicker sample.</summary>
public sealed partial class TimePickerOptionalViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    [NotifyCanExecuteChangedFor(nameof(ClearCommand))]
    public partial TimeSpan? ReminderTime { get; set; }

    [ObservableProperty]
    public partial bool IsPickerOpen { get; set; }

    public string Summary => ReminderTime is { } time
        ? $"We'll remind you at {DateTime.Today.Add(time):t}"
        : "No reminder set";

    [RelayCommand]
    private void Choose() => IsPickerOpen = true;

    [RelayCommand(CanExecute = nameof(HasReminder))]
    private void Clear() => ReminderTime = null;

    private bool HasReminder() => ReminderTime is not null;
}
