namespace MauiGallery.Features.Media;

/// <summary>The .NET methods that the HybridWebView sample's JavaScript calls with <c>InvokeDotNet</c>.</summary>
public sealed class HybridBridge(IDeviceInfo deviceInfo, Action<string> onInvoked)
{
    public string Greet(string name)
    {
        onInvoked($"InvokeDotNet('Greet', ['{name}'])");
        return string.IsNullOrWhiteSpace(name) ? "Hello from .NET!" : $"Hello {name.Trim()}, from .NET {Environment.Version}!";
    }

    public DeviceSummary GetDeviceInfo()
    {
        onInvoked("InvokeDotNet('GetDeviceInfo')");
        return new(deviceInfo.Platform.ToString(), deviceInfo.VersionString, deviceInfo.Model, deviceInfo.Idiom.ToString());
    }
}
