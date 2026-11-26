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

    /// <summary>Refreshes every status, for example when the page appears after a visit to Settings.</summary>
    public async Task CheckAllAsync()
    {
        foreach (var item in Items)
        {
            await item.CheckAsync();
        }

        await VideoCapture.CheckAsync();
    }
}
