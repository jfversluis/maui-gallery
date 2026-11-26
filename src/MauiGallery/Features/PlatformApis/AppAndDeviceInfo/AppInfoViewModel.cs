using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the "App information" sample.</summary>
public sealed partial class AppInfoViewModel(IAppInfo appInfo)
{
    public string Name => appInfo.Name;

    public string PackageName => appInfo.PackageName;

    public string Version => appInfo.VersionString;

    public string Build => appInfo.BuildString;

    public AppTheme Theme => appInfo.RequestedTheme;

    public LayoutDirection LayoutDirection => appInfo.RequestedLayoutDirection;

    [RelayCommand]
    private void ShowSettings() => appInfo.ShowSettingsUI();
}
