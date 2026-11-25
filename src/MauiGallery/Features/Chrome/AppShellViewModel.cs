using CommunityToolkit.Mvvm.Input;
using MauiGallery.Features.Catalog;
using MauiGallery.Services;

namespace MauiGallery.Features.Chrome;

/// <summary>Commands behind the desktop menu bar and its keyboard shortcuts.</summary>
public sealed partial class AppShellViewModel(
    INavigationService navigation,
    IWindowService windows,
    IThemeService theme,
    IBrowser browser)
{
    private const string RepositoryUrl = "https://github.com/jfversluis/maui-gallery";

    public bool CanOpenWindows => windows.CanOpenWindows;

    [RelayCommand]
    private void NewWindow() => windows.OpenNew();

    [RelayCommand]
    private Task GoToAsync(string route) => navigation.GoToAsync(route);

    [RelayCommand]
    private void SetTheme(ThemePreference preference) => theme.Preference = preference;

    [RelayCommand]
    private async Task OpenDocsAsync() => await browser.OpenAsync(new Uri("https://learn.microsoft.com/dotnet/maui/"), BrowserLaunchMode.SystemPreferred);

    [RelayCommand]
    private async Task OpenRepositoryAsync() => await browser.OpenAsync(new Uri(RepositoryUrl), BrowserLaunchMode.SystemPreferred);

    [RelayCommand]
    private async Task ReportIssueAsync() => await browser.OpenAsync(new Uri($"{RepositoryUrl}/issues/new"), BrowserLaunchMode.SystemPreferred);

    public static string HomeRoute => Routes.Home;

    public static string AllControlsRoute => Routes.AllControls;

    public static string SettingsRoute => Routes.Settings;
}
