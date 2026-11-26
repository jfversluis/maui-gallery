using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Collections;

/// <summary>The view model behind the "Generate children from a collection" BindableLayout sample.</summary>
public sealed partial class TagListViewModel : ObservableObject
{
    public ObservableCollection<string> Tags { get; } = ["Beaches", "Hiking", "Street food", "Museums", "Road trips"];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddTagCommand))]
    public partial string NewTag { get; set; } = string.Empty;

    public event EventHandler<string>? TagsChanged;

    [RelayCommand(CanExecute = nameof(CanAddTag))]
    private void AddTag()
    {
        var tag = NewTag.Trim();
        Tags.Add(tag);
        NewTag = string.Empty;
        TagsChanged?.Invoke(this, $"Added \"{tag}\" ({Tags.Count} tags)");
    }

    [RelayCommand]
    private void RemoveTag(string tag)
    {
        Tags.Remove(tag);
        TagsChanged?.Invoke(this, $"Removed \"{tag}\" ({Tags.Count} tags)");
    }

    private bool CanAddTag() =>
        !string.IsNullOrWhiteSpace(NewTag) && !Tags.Contains(NewTag.Trim(), StringComparer.OrdinalIgnoreCase);
}
