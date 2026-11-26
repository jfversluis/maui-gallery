namespace MauiGallery.Features.NativePower;

/// <summary>Operating system details that .NET MAUI doesn't expose.</summary>
public interface INativeOsInfo
{
    /// <summary>The operating system's own version and build string.</summary>
    string GetBuild();

    /// <summary>How hot the device is running, or <see langword="null"/> where the platform doesn't say.</summary>
    string? GetThermalState();
}

/// <summary>
/// Reads operating system details that .NET MAUI doesn't expose. Each platform implements the partial methods in
/// <c>Platforms/&lt;Platform&gt;/NativePower/NativeOsInfo.cs</c>, which the .NET MAUI SDK compiles for that platform only.
/// </summary>
public sealed partial class NativeOsInfo : INativeOsInfo
{
    public string GetBuild() => ReadBuild();

    public string? GetThermalState() => ReadThermalState();

    private static partial string ReadBuild();

    private static partial string? ReadThermalState();
}
