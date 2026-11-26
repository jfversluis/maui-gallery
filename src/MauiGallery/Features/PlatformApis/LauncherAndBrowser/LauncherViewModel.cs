using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the "Open another app" sample.</summary>
public sealed partial class LauncherViewModel(ILauncher launcher) : ObservableObject
{
    [ObservableProperty]
    public partial string Uri { get; set; } = "mauigallery://item/button";

    [ObservableProperty]
    public partial string Result { get; set; } = "Check or open the URI";

    [RelayCommand]
    private async Task CheckAsync()
    {
        if (!TryParse(out var uri))
        {
            return;
        }

        // On iOS and Mac, CanOpenAsync only knows schemes listed under LSApplicationQueriesSchemes in Info.plist.
        Result = await launcher.CanOpenAsync(uri)
            ? $"An app can open {uri.Scheme}: links"
            : $"No app is known to open {uri.Scheme}: links";
    }

    [RelayCommand]
    private async Task OpenAsync()
    {
        if (!TryParse(out var uri))
        {
            return;
        }

        try
        {
            Result = await launcher.OpenAsync(uri) ? "Opened" : "Not opened";
        }
        catch (Exception ex) when (ex is FeatureNotSupportedException or InvalidOperationException)
        {
            Result = $"No app opens {uri.Scheme}: links";
        }
    }

    // TryOpenAsync checks and opens in one call, and returns false instead of throwing.
    [RelayCommand]
    private async Task TryOpenAsync()
    {
        if (TryParse(out var uri))
        {
            Result = await launcher.TryOpenAsync(uri) ? "Opened" : $"No app opens {uri.Scheme}: links";
        }
    }

    private bool TryParse(out Uri uri)
    {
        if (System.Uri.TryCreate(Uri?.Trim(), UriKind.Absolute, out uri!))
        {
            return true;
        }

        Result = "Enter an absolute URI, such as https://dot.net";
        return false;
    }
}
