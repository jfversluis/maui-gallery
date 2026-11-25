using System.Reflection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiGallery.Services;

namespace MauiGallery.Features.Settings;

public sealed partial class SettingsViewModel : ObservableObject, IRecipient<ThemePreferenceChangedMessage>
{
    private const string RepositoryUrl = "https://github.com/jfversluis/maui-gallery";

    private readonly IThemeService _themeService;
    private readonly IBrowser _browser;

    public SettingsViewModel(IThemeService themeService, IBrowser browser, IAppInfo appInfo, IDeviceInfo deviceInfo, IMessenger messenger)
    {
        ArgumentNullException.ThrowIfNull(themeService);
        ArgumentNullException.ThrowIfNull(appInfo);
        ArgumentNullException.ThrowIfNull(deviceInfo);
        ArgumentNullException.ThrowIfNull(messenger);

        _themeService = themeService;
        _browser = browser;
        Theme = themeService.Preference;
        AppVersion = $"{appInfo.VersionString} ({appInfo.BuildString})";
        DotNetVersion = Environment.Version.ToString();
        MauiVersion = InformationalVersion(typeof(MauiApp).Assembly);
        PlatformDescription = $"{deviceInfo.Platform} {deviceInfo.VersionString} · {deviceInfo.Idiom}";
        messenger.Register(this);
    }

    public static IReadOnlyList<ThemePreference> ThemeOptions { get; } = Enum.GetValues<ThemePreference>();

    [ObservableProperty]
    public partial ThemePreference Theme { get; set; }

    public string AppVersion { get; }

    public string DotNetVersion { get; }

    public string MauiVersion { get; }

    public string PlatformDescription { get; }

    public IReadOnlyList<LicenseNotice> Notices { get; } =
    [
        new("Inter", "SIL Open Font License 1.1", new("https://github.com/rsms/inter/blob/master/LICENSE.txt")),
        new("Cascadia Code", "SIL Open Font License 1.1", new("https://github.com/microsoft/cascadia-code/blob/main/LICENSE")),
        new("Fluent UI System Icons", "MIT License", new("https://github.com/microsoft/fluentui-system-icons/blob/main/LICENSE")),
        new("IconFont.Maui.FluentIcons", "MIT License", new("https://github.com/jfversluis/IconFont.Maui.FluentIcons")),
        new("CommunityToolkit.Mvvm", "MIT License", new("https://github.com/CommunityToolkit/dotnet/blob/main/License.md")),
    ];

    /// <summary>Keeps the radio buttons in sync when the theme changes from the menu bar or another window.</summary>
    public void Receive(ThemePreferenceChangedMessage message) => Theme = message.Preference;

    partial void OnThemeChanged(ThemePreference value) => _themeService.Preference = value;

    [RelayCommand]
    private Task OpenRepositoryAsync() => OpenAsync(RepositoryUrl);

    [RelayCommand]
    private Task ReportIssueAsync() => OpenAsync($"{RepositoryUrl}/issues/new");

    [RelayCommand]
    private Task OpenPrivacyPolicyAsync() => OpenAsync($"{RepositoryUrl}/blob/main/docs/privacy.md");

    [RelayCommand]
    private async Task OpenNoticeAsync(LicenseNotice notice) => await _browser.OpenAsync(notice.Url, BrowserLaunchMode.SystemPreferred);

    private async Task OpenAsync(string url) => await _browser.OpenAsync(new Uri(url), BrowserLaunchMode.SystemPreferred);

    private static string InformationalVersion(Assembly assembly)
    {
        var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? assembly.GetName().Version?.ToString()
            ?? "unknown";
        var metadataStart = version.IndexOf('+', StringComparison.Ordinal);
        return metadataStart > 0 ? version[..metadataStart] : version;
    }
}
