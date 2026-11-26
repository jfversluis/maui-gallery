namespace MauiGallery.Features.PlatformApis;

/// <summary>A permission of your own: recording video needs both the camera and the microphone.</summary>
public sealed class CameraAndMicrophone : Permissions.BasePermission
{
    private readonly Permissions.Camera _camera = new();
    private readonly Permissions.Microphone _microphone = new();

    public override async Task<PermissionStatus> CheckStatusAsync() =>
        Combine(await _camera.CheckStatusAsync(), await _microphone.CheckStatusAsync());

    public override async Task<PermissionStatus> RequestAsync()
    {
        var camera = await _camera.RequestAsync();
        if (camera != PermissionStatus.Granted)
        {
            return camera;
        }

        return Combine(camera, await _microphone.RequestAsync());
    }

    public override void EnsureDeclared()
    {
        _camera.EnsureDeclared();
        _microphone.EnsureDeclared();
    }

    public override bool ShouldShowRationale() => _camera.ShouldShowRationale() || _microphone.ShouldShowRationale();

    // Granted only when both are; otherwise the first status that isn't.
    private static PermissionStatus Combine(PermissionStatus camera, PermissionStatus microphone) =>
        camera != PermissionStatus.Granted ? camera : microphone;
}
