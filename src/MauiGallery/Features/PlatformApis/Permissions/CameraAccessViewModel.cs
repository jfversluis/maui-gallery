using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiGallery.Services;

namespace MauiGallery.Features.PlatformApis;

/// <summary>
/// The view model behind the "Explain, ask, then point to Settings" sample: the complete flow for a feature that needs
/// the camera.
/// </summary>
public sealed partial class CameraAccessViewModel(IDialogService dialogs, IAppInfo appInfo) : ObservableObject
{
    [ObservableProperty]
    public partial PermissionStatus Status { get; set; } = PermissionStatus.Unknown;

    [ObservableProperty]
    public partial string Message { get; set; } = "Scanning a code needs the camera.";

    [ObservableProperty]
    public partial bool CanOpenSettings { get; set; }

    [RelayCommand]
    private async Task EnableCameraAsync()
    {
        Status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (Status == PermissionStatus.Granted)
        {
            ShowReady();
            return;
        }

        // Android says when the user has said no before but can still be asked: explain first.
        if (Permissions.ShouldShowRationale<Permissions.Camera>()
            && !await dialogs.ConfirmAsync("Use the camera?", "The camera reads the code. Nothing is recorded or saved.", "Continue", "Not now"))
        {
            Message = "Okay, no camera for now.";
            return;
        }

        Status = await Permissions.RequestAsync<Permissions.Camera>();
        if (Status == PermissionStatus.Granted)
        {
            ShowReady();
            return;
        }

        // Asking again won't show a prompt any more. Offer Settings instead, and say why.
        Message = "Camera access is off. Turn it on in Settings to scan codes.";
        CanOpenSettings = true;
        SemanticScreenReader.Announce(Message);
    }

    [RelayCommand]
    private void OpenSettings() => appInfo.ShowSettingsUI();

    private void ShowReady()
    {
        Message = "The camera is ready to scan.";
        CanOpenSettings = false;
    }
}
