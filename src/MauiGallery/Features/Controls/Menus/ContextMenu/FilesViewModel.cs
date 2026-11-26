using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Menus;

/// <summary>A file in the "Submenus and commands in a list" context menu sample.</summary>
public sealed partial class FileItem(string name, string folder) : ObservableObject
{
    public string Name { get; } = name;

    [ObservableProperty]
    public partial string Folder { get; set; } = folder;

    /// <summary>Raised when the file moves, so the list can report it.</summary>
    public event EventHandler? Moved;

    [RelayCommand]
    private void MoveTo(string folder)
    {
        Folder = folder;
        Moved?.Invoke(this, EventArgs.Empty);
    }
}

/// <summary>The view model behind the "Submenus and commands in a list" context menu sample.</summary>
public sealed partial class FilesViewModel : ObservableObject
{
    public FilesViewModel() => Restore();

    public ObservableCollection<FileItem> Files { get; } = [];

    /// <summary>Raised with a description of each action, so the gallery can show it in the sample's output.</summary>
    public event EventHandler<string>? ActionInvoked;

    [RelayCommand]
    private void Open(FileItem file) => ActionInvoked?.Invoke(this, $"OpenCommand: {file.Name}");

    [RelayCommand]
    private void Delete(FileItem file)
    {
        Files.Remove(file);
        ActionInvoked?.Invoke(this, $"DeleteCommand: {file.Name}");
    }

    [RelayCommand]
    private void Restore()
    {
        Files.Clear();
        foreach (var file in new FileItem[] { new("Budget.xlsx", "Documents"), new("Trip notes.txt", "Desktop"), new("Invoice 1042.pdf", "Downloads") })
        {
            file.Moved += (_, _) => ActionInvoked?.Invoke(this, $"MoveToCommand: {file.Name} → {file.Folder}");
            Files.Add(file);
        }
    }
}
