using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Layouts;

/// <summary>The tags shown by the FlexLayout "chip cloud" sample.</summary>
public sealed partial class TagCloudViewModel : ObservableObject
{
    public ObservableCollection<string> Tags { get; } = ["MAUI", "C#", "XAML", "Android", "iOS", "Windows", "macOS", "Hot Reload", "Blazor"];

    [ObservableProperty]
    public partial string? NewTag { get; set; }

    [RelayCommand]
    private void Add()
    {
        var tag = NewTag?.Trim();
        if (!string.IsNullOrEmpty(tag) && !Tags.Contains(tag))
        {
            Tags.Add(tag);
        }

        NewTag = string.Empty;
    }

    [RelayCommand]
    private void Remove(string tag) => Tags.Remove(tag);
}
