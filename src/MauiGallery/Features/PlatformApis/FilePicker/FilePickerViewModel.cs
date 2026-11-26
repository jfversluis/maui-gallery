using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the "Pick a file" and "Pick several files" samples.</summary>
public sealed partial class FilePickerViewModel(IFilePicker filePicker) : ObservableObject
{
    private const int MaxPreviewCharacters = 400;
    private const long MaxPreviewImageBytes = 20 * 1024 * 1024;

    // Each platform names file types its own way: UTIs on Apple platforms, MIME types on Android, extensions on Windows.
    public static FilePickerFileType TextFiles { get; } = new(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        [DevicePlatform.iOS] = ["public.plain-text"],
        [DevicePlatform.MacCatalyst] = ["public.plain-text"],
        [DevicePlatform.Android] = ["text/plain"],
        [DevicePlatform.WinUI] = [".txt"],
    });

    public static IReadOnlyList<string> FileTypeNames { get; } = ["Any file", "Images", "PDF", "Text"];

    [ObservableProperty]
    public partial string FileTypeName { get; set; } = "Any file";

    [ObservableProperty]
    public partial string Status { get; set; } = "No file picked";

    [ObservableProperty]
    public partial string? FileName { get; set; }

    [ObservableProperty]
    public partial string? ContentType { get; set; }

    [ObservableProperty]
    public partial string? Size { get; set; }

    [ObservableProperty]
    public partial string? TextPreview { get; set; }

    [ObservableProperty]
    public partial ImageSource? ImagePreview { get; set; }

    public ObservableCollection<string> PickedFiles { get; } = [];

    private FilePickerFileType? FileTypes => FileTypeName switch
    {
        "Images" => FilePickerFileType.Images,
        "PDF" => FilePickerFileType.Pdf,
        "Text" => TextFiles,
        _ => null, // Any file
    };

    [RelayCommand]
    private async Task PickAsync()
    {
        try
        {
            var file = await filePicker.PickAsync(new PickOptions { PickerTitle = "Choose a file", FileTypes = FileTypes });
            if (file is null)
            {
                Status = "Picking was cancelled";
                return;
            }

            await ShowAsync(file);
        }
        catch (Exception ex) when (ex is PermissionException or FeatureNotSupportedException or IOException)
        {
            Status = $"Couldn't pick a file: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task PickImagesAsync()
    {
        try
        {
            var files = await filePicker.PickMultipleAsync(new PickOptions { PickerTitle = "Choose images", FileTypes = FilePickerFileType.Images });
            PickedFiles.Clear();
            foreach (var file in files ?? [])
            {
                PickedFiles.Add($"{file.FileName} ({file.ContentType})");
            }
        }
        catch (Exception ex) when (ex is PermissionException or FeatureNotSupportedException)
        {
            PickedFiles.Clear();
            PickedFiles.Add($"Couldn't pick files: {ex.Message}");
        }
    }

    private async Task ShowAsync(FileResult file)
    {
        Status = "Opening…";
        FileName = file.FileName;
        ContentType = file.ContentType;
        TextPreview = null;
        ImagePreview = null;

        // The picked file may be a temporary copy: read it now, or copy it to AppDataDirectory to keep it.
        // It can be any size, so read only what the preview needs.
        await using var stream = await file.OpenReadAsync();
        long? length = stream.CanSeek ? stream.Length : null;
        Size = length switch
        {
            null => "Unknown",
            < 1024 => $"{length} bytes",
            < 1024 * 1024 => $"{length / 1024.0:N1} KB",
            _ => $"{length / (1024.0 * 1024):N1} MB",
        };
        Status = "Picked";

        if (file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            // Decoding a huge image could exhaust memory; the image loads the file itself, when it's shown.
            if (length <= MaxPreviewImageBytes)
            {
                ImagePreview = ImageSource.FromStream(_ => file.OpenReadAsync());
            }
            else
            {
                TextPreview = "Too large to preview";
            }
        }
        else if (file.ContentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase)
            || file.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
            || file.FileName.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
        {
            // Read one character more than the preview shows, to know whether there's more.
            using var reader = new StreamReader(stream);
            var buffer = new char[MaxPreviewCharacters + 1];
            var read = await reader.ReadBlockAsync(buffer);
            TextPreview = read > MaxPreviewCharacters ? new string(buffer, 0, MaxPreviewCharacters) + "…" : new string(buffer, 0, read);
        }
    }
}
