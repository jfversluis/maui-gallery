using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Fundamentals;

/// <summary>A tile in the "Reorder tiles" drag and drop sample.</summary>
public sealed partial class ReorderTile(string name, Color color) : ObservableObject
{
    public string Name { get; } = name;

    public Color Color { get; } = color;

    /// <summary>Whether a dragged tile is over this one, to highlight where it will land.</summary>
    [ObservableProperty]
    public partial bool IsDragOver { get; set; }
}

/// <summary>The view model behind the "Reorder tiles" drag and drop sample.</summary>
public sealed partial class ReorderViewModel : ObservableObject
{
    private ReorderTile? _dragged;

    public ObservableCollection<ReorderTile> Tiles { get; } =
    [
        new("Inbox", Color.FromArgb("#512BD4")),
        new("Calendar", Color.FromArgb("#2563EB")),
        new("Photos", Color.FromArgb("#0D9488")),
        new("Music", Color.FromArgb("#EA580C")),
        new("Notes", Color.FromArgb("#DB2777")),
    ];

    /// <summary>A summary of the current order, such as "Inbox, Calendar, Photos".</summary>
    public string Order => string.Join(", ", Tiles.Select(t => t.Name));

    [RelayCommand]
    private void DragStarting(ReorderTile tile) => _dragged = tile;

    // Highlights the tile under the pointer, unless it's the one being dragged.
    [RelayCommand]
    private void DragOver(ReorderTile tile)
    {
        foreach (var item in Tiles)
        {
            item.IsDragOver = item == tile && item != _dragged;
        }
    }

    [RelayCommand]
    private void DragLeave()
    {
        foreach (var item in Tiles)
        {
            item.IsDragOver = false;
        }
    }

    [RelayCommand]
    private void Drop(ReorderTile target)
    {
        target.IsDragOver = false;
        if (_dragged is null || _dragged == target)
        {
            return;
        }

        Tiles.Move(Tiles.IndexOf(_dragged), Tiles.IndexOf(target));
        _dragged = null;
        OnPropertyChanged(nameof(Order));
    }
}
