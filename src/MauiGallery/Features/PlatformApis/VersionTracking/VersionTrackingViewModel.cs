using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the Version tracking samples.</summary>
public sealed partial class VersionTrackingViewModel(IVersionTracking versionTracking) : ObservableObject
{
    public bool IsFirstLaunchEver => versionTracking.IsFirstLaunchEver;

    public bool IsFirstLaunchForCurrentVersion => versionTracking.IsFirstLaunchForCurrentVersion;

    public bool IsFirstLaunchForCurrentBuild => versionTracking.IsFirstLaunchForCurrentBuild;

    public string CurrentVersion => versionTracking.CurrentVersion;

    public string CurrentBuild => versionTracking.CurrentBuild;

    public string PreviousVersion => versionTracking.PreviousVersion ?? "None";

    public string PreviousBuild => versionTracking.PreviousBuild ?? "None";

    public string FirstInstalledVersion => versionTracking.FirstInstalledVersion ?? "None";

    public string FirstInstalledBuild => versionTracking.FirstInstalledBuild ?? "None";

    public string VersionHistory => string.Join(", ", versionTracking.VersionHistory);

    public string BuildHistory => string.Join(", ", versionTracking.BuildHistory);

    public string Welcome =>
        IsFirstLaunchEver ? "Welcome! Here's a quick tour."
        : IsFirstLaunchForCurrentVersion ? $"What's new in {CurrentVersion}"
        : "Welcome back";

    /// <summary>A version to look up in the history.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(VersionCheck))]
    public partial string VersionToCheck { get; set; } = "1.0.0";

    public string VersionCheck =>
        versionTracking.VersionHistory.Contains(VersionToCheck.Trim(), StringComparer.Ordinal)
            ? $"Version {VersionToCheck.Trim()} has run on this device"
            : $"Version {VersionToCheck.Trim()} has never run here";
}
