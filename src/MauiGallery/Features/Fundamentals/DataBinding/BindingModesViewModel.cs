using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Fundamentals;

/// <summary>The single source behind the "Binding modes, side by side" sample.</summary>
public sealed partial class BindingModesViewModel : ObservableObject
{
    private static readonly string[] Names = ["Grace Hopper", "Katherine Johnson", "Ada Lovelace"];

    private int _next;

    [ObservableProperty]
    public partial string? Name { get; set; } = "Ada Lovelace";

    [RelayCommand]
    private void ChangeSource()
    {
        Name = Names[_next];
        _next = (_next + 1) % Names.Length;
    }
}
