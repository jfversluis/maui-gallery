using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Menus;

/// <summary>The view model behind the "Enable items with a command" toolbar sample.</summary>
public sealed partial class NoteViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial string? Text { get; set; }

    /// <summary>Raised with the saved note, so the gallery can show it in the sample's output.</summary>
    public event EventHandler<string>? Saved;

    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        Saved?.Invoke(this, Text!);
        Text = null;
    }

    private bool CanSave() => !string.IsNullOrWhiteSpace(Text);
}
