using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class VersionTrackingPage : SamplePage
{
    public VersionTrackingPage(SamplePageServices services, VersionTrackingViewModel versionTracking)
        : base(services)
    {
        InitializeComponent();

        LaunchSample.BindingContext = versionTracking;
        HistorySample.BindingContext = versionTracking;
        HistoryOptions.BindingContext = versionTracking;
        LaunchExample.Output = $"Launches recorded for versions {versionTracking.VersionHistory}";
        versionTracking.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(VersionTrackingViewModel.VersionCheck))
            {
                HistoryExample.Output = versionTracking.VersionCheck;
            }
        };
    }
}
