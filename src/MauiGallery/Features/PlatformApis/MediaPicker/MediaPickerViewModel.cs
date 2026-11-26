using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the Media picker samples: pick or take photos and videos.</summary>
/// <remarks>
/// It shows photos straight from the picker's files and copies nothing, so the app keeps no media of its own. The
/// picker's files are temporary: an app that keeps a photo copies it to <see cref="FileSystem.AppDataDirectory"/>.
/// </remarks>
public sealed partial class MediaPickerViewModel(IMediaPicker mediaPicker) : ObservableObject
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
    public partial ImageSource? Photo { get; set; }

    public bool HasNoPhoto => Photo is null;

    [ObservableProperty]
    public partial string PhotoInfo { get; set; } = string.Empty;

    // Pick several photos
    [ObservableProperty]
    public partial double SelectionLimit { get; set; } = 4;

    public ObservableCollection<ImageSource> Photos { get; } = [];

    [ObservableProperty]
    public partial string PhotosInfo { get; set; } = string.Empty;

    // Take a photo
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNoCapture))]
    public partial ImageSource? CapturedPhoto { get; set; }

    public bool HasNoCapture => CapturedPhoto is null;

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
            Photo = Preview(result);
            PhotoInfo = await DescribeAsync(result);
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

        Photos.Clear();
        foreach (var result in results)
        {
            Photos.Add(Preview(result));
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
            CapturedPhoto = Preview(result);
            CaptureInfo = await DescribeAsync(result);
        }
    }

    [RelayCommand]
    private async Task PickVideoAsync()
    {
        var result = (await RunAsync(() => mediaPicker.PickVideosAsync(new MediaPickerOptions { SelectionLimit = 1 }), info => VideoInfo = info)).FirstOrDefault();
        if (result is not null)
        {
            VideoInfo = await DescribeAsync(result);
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
            VideoInfo = await DescribeAsync(result);
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

    // The image opens the picker's file when it loads, and keeps the FileResult alive as long as it's shown.
    private static ImageSource Preview(FileResult file) => ImageSource.FromStream(_ => file.OpenReadAsync());

    // Opening the file doesn't read it, so this is cheap even for a long video.
    private static async Task<string> DescribeAsync(FileResult file)
    {
        try
        {
            await using var stream = await file.OpenReadAsync();
            var size = stream.CanSeek ? $"{stream.Length / 1024.0:N0} KB" : "size unknown";
            return $"{file.FileName} · {size} · {file.ContentType}";
        }
        catch (IOException)
        {
            return $"{file.FileName} · {file.ContentType} · couldn't be read";
        }
    }
}
