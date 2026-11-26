using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the Media picker samples: pick or take photos and videos.</summary>
public sealed partial class MediaPickerViewModel(IMediaPicker mediaPicker, IFileSystem fileSystem) : ObservableObject
{
    public bool IsCaptureSupported { get; } = mediaPicker.IsCaptureSupported;

    // Pick a photo
    /// <summary>The longest side to scale picked photos down to, or <see langword="null"/> for the original size.</summary>
    [ObservableProperty]
    public partial int? MaximumSize { get; set; }

    [ObservableProperty]
    public partial double CompressionQuality { get; set; } = 100;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNoPhoto))]
    public partial string? PhotoPath { get; set; }

    public bool HasNoPhoto => PhotoPath is null;

    [ObservableProperty]
    public partial string PhotoInfo { get; set; } = string.Empty;

    // Pick several photos
    [ObservableProperty]
    public partial double SelectionLimit { get; set; } = 4;

    public ObservableCollection<string> PhotoPaths { get; } = [];

    [ObservableProperty]
    public partial string PhotosInfo { get; set; } = string.Empty;

    // Take a photo
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNoCapture))]
    public partial string? CapturedPath { get; set; }

    public bool HasNoCapture => CapturedPath is null;

    [ObservableProperty]
    public partial string CaptureInfo { get; set; } = mediaPicker.IsCaptureSupported ? string.Empty : "This device has no camera.";

    // Videos
    [ObservableProperty]
    public partial string VideoInfo { get; set; } = "No video yet";

    [RelayCommand]
    private async Task PickPhotoAsync()
    {
        // PickPhotoAsync is obsolete since .NET 11: PickPhotosAsync with a SelectionLimit of 1 replaces it.
        var options = new MediaPickerOptions
        {
            Title = "Choose a photo",
            SelectionLimit = 1,
            MaximumWidth = MaximumSize,
            MaximumHeight = MaximumSize,
            CompressionQuality = (int)Math.Round(CompressionQuality),
        };

        var result = (await RunAsync(() => mediaPicker.PickPhotosAsync(options), info => PhotoInfo = info)).FirstOrDefault();
        if (result is not null)
        {
            PhotoPath = await CopyToCacheAsync(result);
            PhotoInfo = Describe(result, PhotoPath);
        }
    }

    [RelayCommand]
    private async Task PickPhotosAsync()
    {
        var options = new MediaPickerOptions { SelectionLimit = (int)Math.Round(SelectionLimit), MaximumWidth = 600, MaximumHeight = 600 };
        var results = await RunAsync(() => mediaPicker.PickPhotosAsync(options), info => PhotosInfo = info);
        if (results.Count == 0)
        {
            return;
        }

        PhotoPaths.Clear();
        foreach (var result in results)
        {
            PhotoPaths.Add(await CopyToCacheAsync(result));
        }

        PhotosInfo = results.Count == 1 ? "1 photo" : $"{results.Count} photos";
    }

    [RelayCommand]
    private async Task CapturePhotoAsync()
    {
        if (!IsCaptureSupported)
        {
            CaptureInfo = "This device has no camera.";
            return;
        }

        var result = await RunAsync(() => mediaPicker.CapturePhotoAsync(new MediaPickerOptions { MaximumWidth = 1600, MaximumHeight = 1600 }), info => CaptureInfo = info);
        if (result is not null)
        {
            CapturedPath = await CopyToCacheAsync(result);
            CaptureInfo = Describe(result, CapturedPath);
        }
    }

    [RelayCommand]
    private async Task PickVideoAsync()
    {
        var result = (await RunAsync(() => mediaPicker.PickVideosAsync(new MediaPickerOptions { SelectionLimit = 1 }), info => VideoInfo = info)).FirstOrDefault();
        if (result is not null)
        {
            VideoInfo = Describe(result, await CopyToCacheAsync(result));
        }
    }

    [RelayCommand]
    private async Task CaptureVideoAsync()
    {
        if (!IsCaptureSupported)
        {
            VideoInfo = "This device has no camera.";
            return;
        }

        var result = await RunAsync(() => mediaPicker.CaptureVideoAsync(), info => VideoInfo = info);
        if (result is not null)
        {
            VideoInfo = Describe(result, await CopyToCacheAsync(result));
        }
    }

    // A null result or an empty list means the user cancelled, which is a normal choice, not an error.
    private static async Task<FileResult?> RunAsync(Func<Task<FileResult?>> pick, Action<string> report)
    {
        var results = await RunAsync(async () => await pick() is { } result ? new List<FileResult> { result } : new List<FileResult>(), report);
        return results.FirstOrDefault();
    }

    private static async Task<List<FileResult>> RunAsync(Func<Task<List<FileResult>>> pick, Action<string> report)
    {
        try
        {
            var results = await pick();
            if (results.Count == 0)
            {
                report("Cancelled");
            }

            return results;
        }
        catch (FeatureNotSupportedException)
        {
            report("Not supported on this device.");
        }
        catch (PermissionException)
        {
            report("Camera access is off. Turn it on in Settings to take photos and videos.");
        }

        return [];
    }

    // The picker's file can be temporary: copy it to storage the app owns before using it.
    private async Task<string> CopyToCacheAsync(FileResult file)
    {
        var path = Path.Combine(fileSystem.CacheDirectory, $"{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}");
        await using var source = await file.OpenReadAsync();
        await using var target = File.Create(path);
        await source.CopyToAsync(target);
        return path;
    }

    private static string Describe(FileResult file, string copy) =>
        $"{file.FileName} · {new FileInfo(copy).Length / 1024.0:N0} KB · {file.ContentType}";
}
