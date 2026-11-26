using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the Clipboard samples.</summary>
public sealed partial class ClipboardViewModel(IClipboard clipboard) : ObservableObject
{
    [ObservableProperty]
    public partial string Text { get; set; } = "Hello from .NET MAUI";

    [ObservableProperty]
    public partial string Pasted { get; set; } = "Nothing pasted yet";

    [ObservableProperty]
    public partial bool HasText { get; set; } = clipboard.HasText;

    [ObservableProperty]
    public partial int ChangeCount { get; set; }

    /// <summary>Starts listening for clipboard changes. Call it when the page appears.</summary>
    public void Start()
    {
        HasText = clipboard.HasText;
        clipboard.ClipboardContentChanged += OnClipboardContentChanged;
    }

    /// <summary>Stops listening. Call it when the page disappears.</summary>
    public void Stop() => clipboard.ClipboardContentChanged -= OnClipboardContentChanged;

    [RelayCommand]
    private Task CopyAsync() => clipboard.SetTextAsync(Text);

    [RelayCommand]
    private async Task PasteAsync() =>
        Pasted = clipboard.HasText ? await clipboard.GetTextAsync() ?? string.Empty : "The clipboard has no text";

    [RelayCommand]
    private Task ClearAsync() => clipboard.SetTextAsync(null);

    private void OnClipboardContentChanged(object? sender, EventArgs e)
    {
        HasText = clipboard.HasText;
        ChangeCount++;
    }
}
