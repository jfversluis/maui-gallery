using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Pickers;

/// <summary>The view model behind the "Start empty and open from code" DatePicker sample.</summary>
public sealed partial class DatePickerOptionalViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    [NotifyCanExecuteChangedFor(nameof(ClearCommand))]
    public partial DateTime? DueDate { get; set; }

    [ObservableProperty]
    public partial bool IsPickerOpen { get; set; }

    public string Summary => DueDate is not { } date ? "No due date" : (date.Date - DateTime.Today).Days switch
    {
        0 => "Due today",
        1 => "Due tomorrow",
        < 0 and var days => $"Overdue by {-days} days",
        var days => $"Due in {days} days",
    };

    [RelayCommand]
    private void Choose() => IsPickerOpen = true;

    [RelayCommand(CanExecute = nameof(HasDueDate))]
    private void Clear() => DueDate = null;

    private bool HasDueDate() => DueDate is not null;
}
