using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the "Check", "Request" and "Your own permission" samples.</summary>
public sealed partial class PermissionsViewModel : ObservableObject
{
    public PermissionsViewModel()
    {
        Items =
        [
            new("Location (when in use)", new Permissions.LocationWhenInUse()),
            new("Camera", new Permissions.Camera()),
            new("Microphone", new Permissions.Microphone()),
        ];
        Selected = Items[0];
    }

    public IReadOnlyList<PermissionItem> Items { get; }

    [ObservableProperty]
    public partial PermissionItem Selected { get; set; }

    public PermissionItem VideoCapture { get; } = new("Camera and microphone", new CameraAndMicrophone());

    /// <summary>
    /// On Windows, checking the location permission calls Geolocator.RequestAccessAsync, which can show the system's
    /// location prompt. So when the page appears it skips location there, and only "Check again" reads it.
    /// </summary>
    public static bool CheckingLocationPrompts => DeviceInfo.Platform == DevicePlatform.WinUI;

    /// <summary>Refreshes every status, for example when the page appears after a visit to Settings.</summary>
    /// <param name="userInitiated">False when the page refreshes by itself; then no check may show a prompt.</param>
    public async Task CheckAllAsync(bool userInitiated = true)
    {
        foreach (var item in Items)
        {
            if (!userInitiated && CheckingLocationPrompts && item.TypeName == nameof(Permissions.LocationWhenInUse))
            {
                continue;
            }

            await item.CheckAsync();
        }

        await VideoCapture.CheckAsync();
    }
}
