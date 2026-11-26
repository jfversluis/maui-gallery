using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Fundamentals;

/// <summary>The page-level view model of the "Bind to an ancestor" sample. Each item's button calls its command.</summary>
public sealed partial class ShoppingListViewModel : ObservableObject
{
    private static readonly string[] DefaultItems = ["Apples", "Bread", "Coffee", "Milk"];

    public ObservableCollection<string> Items { get; } = [.. DefaultItems];

    [RelayCommand]
    private void Remove(string item) => Items.Remove(item);

    [RelayCommand]
    private void Reset()
    {
        Items.Clear();
        foreach (var item in DefaultItems)
        {
            Items.Add(item);
        }
    }
}
