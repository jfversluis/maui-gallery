using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>One permission, its last known status, and commands to check and request it.</summary>
public sealed partial class PermissionItem(string name, Permissions.BasePermission permission) : ObservableObject
{
    public string Name { get; } = name;

    /// <summary>The permission's class name, as you write it in code.</summary>
    public string TypeName { get; } = permission.GetType().Name;

    [ObservableProperty]
    public partial PermissionStatus Status { get; set; } = PermissionStatus.Unknown;

    /// <summary>Whether Android suggests explaining why the app needs the permission before asking again.</summary>
    [ObservableProperty]
    public partial bool ShouldShowRationale { get; set; }

    [ObservableProperty]
    public partial string? Message { get; set; }

    /// <summary>Reads the status. Checking never shows a prompt, so it's safe to do whenever a page appears.</summary>
    [RelayCommand]
    public Task CheckAsync() => RunAsync(permission.CheckStatusAsync);

    [RelayCommand]
    private async Task RequestAsync()
    {
        await RunAsync(permission.RequestAsync);
        Message = Status switch
        {
            PermissionStatus.Granted => "Granted",
            PermissionStatus.Limited => "Granted, with limits the user chose",
            PermissionStatus.Denied when ShouldShowRationale => "Denied. Explain why before asking again.",
            PermissionStatus.Denied => "Denied. Asking again won't show a prompt; only Settings can change it.",
            _ => Message,
        };
    }

    private async Task RunAsync(Func<Task<PermissionStatus>> action)
    {
        try
        {
            Status = await action();
            ShouldShowRationale = permission.ShouldShowRationale();
            Message = null;
        }
        catch (PermissionException ex)
        {
            // Thrown when the permission isn't declared in AndroidManifest.xml or Info.plist.
            Message = ex.Message;
        }
        catch (FeatureNotSupportedException)
        {
            Message = "Not available on this device";
        }
    }
}
