using MauiGallery.Features.Catalog;
using MauiGallery.Features.Samples;
using MauiGallery.Services;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class AppThemePage : SamplePage
{
    private readonly INavigationService _navigation;
    private Application? _application;

    public AppThemePage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        _navigation = services.Navigation;
        ReactExample.NativeTarget = ReactSettingsButton;
        SwapTheme.SelectedIndex = 0;
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        _application = Application.Current;
        if (_application is not null)
        {
            _application.RequestedThemeChanged += OnRequestedThemeChanged;
        }

        ShowThemes();
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        if (_application is not null)
        {
            _application.RequestedThemeChanged -= OnRequestedThemeChanged;
            _application = null;
        }
    }

    private void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e)
    {
        ShowThemes();
        ReactExample.Output = $"RequestedThemeChanged: {e.RequestedTheme}";
    }

    private void ShowThemes()
    {
        if (_application is null)
        {
            return;
        }

        ReactRequested.Text = _application.RequestedTheme.ToString();
        ReactUser.Text = _application.UserAppTheme.ToString();
        ReactPlatform.Text = _application.PlatformAppTheme.ToString();
    }

    private async void OnOpenSettingsClicked(object? sender, EventArgs e) =>
        await _navigation.GoToAsync(Routes.Settings);

    private void OnSwapThemeChanged(object? sender, EventArgs e)
    {
        var merged = SwapSample.Resources.MergedDictionaries;
        ResourceDictionary next = SwapTheme.SelectedIndex == 1 ? new ThemePreviewDark() : new ThemePreviewLight();
        if (merged.FirstOrDefault()?.GetType() == next.GetType())
        {
            return;
        }

        // Add the new colors before removing the old ones, so no DynamicResource ever sees a missing key.
        var current = merged.FirstOrDefault();
        merged.Add(next);
        if (current is not null)
        {
            merged.Remove(current);
        }

        SwapExample.Output = $"MergedDictionaries: {next.GetType().Name}";
    }
}
