using IconFont.Maui.FluentIcons;

namespace MauiGallery.Features.Media;

/// <summary>
/// Takes a map's place on the stage when this build has no key for the platform's map service. On Windows, where the
/// key can be set at runtime, it also lets people paste their own.
/// </summary>
public sealed partial class MapUnavailableView : ContentView
{
    private static readonly Uri KeyHelp = new("https://learn.microsoft.com/azure/azure-maps/how-to-manage-account-keys");

    private readonly IBrowser _browser;

    public MapUnavailableView(IBrowser browser)
    {
        InitializeComponent();
        _browser = browser;
        IconLabel.Text = FluentIconsRegular.Key24;

        // The key form needs the room on a phone-sized stage, so it replaces the icon.
        IconLabel.IsVisible = !MapAvailability.AcceptsRuntimeKey;
        KeyForm.IsVisible = MapAvailability.AcceptsRuntimeKey;
        (TitleLabel.Text, MessageLabel.Text) = MapAvailability.AcceptsRuntimeKey
            ? ("Map needs an Azure Maps key",
                "The Windows map renders Azure Maps, and this build has no key. Paste your own to try it; the gallery forgets it when you close the app.")
            : ("Map needs an API key",
                "Google Maps on Android needs an API key in the app manifest, and this build has none. Build the gallery with -p:MapsApiKey=<your key> to see the map.");
        SemanticProperties.SetDescription(this, $"{TitleLabel.Text}. {MessageLabel.Text}");
    }

    /// <summary>Raised once a pasted key has been applied, so the page can show its maps.</summary>
    public event EventHandler? KeyApplied;

    private void OnUseKey(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(KeyEntry.Text))
        {
            KeyEntry.Focus();
            return;
        }

        MapAvailability.UseRuntimeKey(KeyEntry.Text.Trim());
        KeyApplied?.Invoke(this, EventArgs.Empty);
    }

    private async void OnHelpClicked(object? sender, EventArgs e) =>
        await _browser.OpenAsync(KeyHelp, BrowserLaunchMode.SystemPreferred);
}
