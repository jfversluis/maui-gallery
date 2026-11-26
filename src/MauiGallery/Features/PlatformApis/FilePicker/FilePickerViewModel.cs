using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the "Pick a file" and "Pick several files" samples.</summary>
public sealed partial class FilePickerViewModel(IFilePicker filePicker) : ObservableObject
{
    private const int MaxPreviewCharacters = 400;

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
        // The picked file may be a temporary copy: read it now, or copy it to AppDataDirectory to keep it.
        await using var stream = await file.OpenReadAsync();
        using var memory = new MemoryStream();
        await stream.CopyToAsync(memory);
        var bytes = memory.ToArray();

        FileName = file.FileName;
        ContentType = file.ContentType;
        Size = bytes.Length < 1024 ? $"{bytes.Length} bytes" : $"{bytes.Length / 1024.0:N1} KB";
        Status = "Picked";

        var isImage = file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
        var isText = file.ContentType.StartsWith("text/", StringComparison.OrdinalIgnoreCase)
            || file.FileName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)
            || file.FileName.EndsWith(".md", StringComparison.OrdinalIgnoreCase);

        ImagePreview = isImage ? ImageSource.FromStream(() => new MemoryStream(bytes)) : null;
        if (isText)
        {
            using var reader = new StreamReader(new MemoryStream(bytes));
            var text = await reader.ReadToEndAsync();
            TextPreview = text.Length > MaxPreviewCharacters ? text[..MaxPreviewCharacters] + "…" : text;
        }
        else
        {
            TextPreview = null;
        }
    }
}
