using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiGallery.Services;

namespace MauiGallery.Features.Controls.Dialogs;

/// <summary>The view model behind the "Confirm from a view model" alerts sample.</summary>
public sealed partial class DraftsViewModel(IDialogService dialogs) : ObservableObject
{
    private const int InitialDrafts = 3;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    [NotifyCanExecuteChangedFor(nameof(DeleteAllCommand))]
    public partial int DraftCount { get; set; } = InitialDrafts;

    public string Summary => DraftCount switch
    {
        0 => "No drafts",
        1 => "1 draft",
        _ => $"{DraftCount} drafts",
    };

    /// <summary>Raised with the user's answer, so the gallery can show it in the sample's output.</summary>
    public event EventHandler<bool>? Confirmed;

    [RelayCommand(CanExecute = nameof(HasDrafts))]
    private async Task DeleteAllAsync()
    {
        var confirmed = await dialogs.ConfirmAsync(
            "Delete all drafts?",
            $"{Summary} will be deleted. You can't undo this.",
            "Delete",
            "Cancel");
        Confirmed?.Invoke(this, confirmed);

        if (confirmed)
        {
            DraftCount = 0;
        }
    }

    private bool HasDrafts() => DraftCount > 0;

    [RelayCommand]
    private void Restore() => DraftCount = InitialDrafts;
}
