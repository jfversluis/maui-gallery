using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class LauncherAndBrowserPage : SamplePage
{
    private readonly IBrowser _browser;

    public LauncherAndBrowserPage(SamplePageServices services, IBrowser browser, LauncherViewModel launcher, MapViewModel map)
        : base(services)
    {
        _browser = browser;
        InitializeComponent();

        BrowserMode.ItemsSource = Enum.GetValues<BrowserLaunchMode>();
        BrowserMode.SelectedItem = BrowserLaunchMode.SystemPreferred;

        LauncherSample.BindingContext = launcher;
        LauncherPreset.SelectedItem = launcher.Uri;
        LauncherExample.NativeTarget = LauncherOpenButton;
        launcher.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(LauncherViewModel.Result))
            {
                LauncherExample.Output = launcher.Result;
            }
        };

        MapSample.BindingContext = map;
        MapOptions.BindingContext = map;
        MapExample.NativeTarget = MapButton;
        map.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(MapViewModel.NavigationMode):
                    MapModeSubstitution.Value = $"NavigationMode.{map.NavigationMode}";
                    break;
                case nameof(MapViewModel.UseAddress):
                    MapTargetSubstitution.Value = map.UseAddress ? "address" : "location";
                    break;
                case nameof(MapViewModel.Result):
                    MapExample.Output = $"TryOpenAsync: {map.Result}";
                    break;
            }
        };
    }

    private async void OnOpenBrowserClicked(object? sender, EventArgs e)
    {
        if (!Uri.TryCreate(BrowserUri.Text?.Trim(), UriKind.Absolute, out var uri))
        {
            BrowserExample.Output = "Enter an absolute URI, such as https://dot.net";
            return;
        }

        var opened = await _browser.OpenAsync(uri, new BrowserLaunchOptions
        {
            LaunchMode = (BrowserLaunchMode)BrowserMode.SelectedItem,
            PreferredToolbarColor = BrowserToolbar.SelectedColor,
            PreferredControlColor = BrowserControl.SelectedColor,
        });
        BrowserExample.Output = $"OpenAsync returned {opened}";
    }

    private void OnBrowserModeChanged(object? sender, EventArgs e) =>
        BrowserModeSubstitution.Value = $"BrowserLaunchMode.{BrowserMode.SelectedItem}";

    private void OnLauncherPresetChanged(object? sender, EventArgs e)
    {
        if (LauncherPreset.SelectedItem is string uri && LauncherSample.BindingContext is LauncherViewModel launcher)
        {
            launcher.Uri = uri;
        }
    }
}
