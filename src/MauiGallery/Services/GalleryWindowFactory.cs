namespace MauiGallery.Services;

/// <summary>Creates app windows. Each window gets its own Shell, so windows navigate independently.</summary>
public sealed class GalleryWindowFactory(
    IServiceProvider services,
    ActiveWindowTracker windows,
    INavigationService navigation,
    DeepLinkRouter deepLinks)
{
    public Window Create(string? initialItemId = null)
    {
        // Resolving per window is intentional: AppShell and its sidebar are window-scoped.
        var shell = services.GetRequiredService<AppShell>();
        var window = new Window(shell) { Title = ".NET MAUI Gallery" };
        window.SizeChanged += (_, _) => shell.ApplyLayout(window.Width);
        windows.Track(window);

        shell.Loaded += OnShellLoaded;
        return window;

        async void OnShellLoaded(object? sender, EventArgs e)
        {
            shell.Loaded -= OnShellLoaded;
            if (initialItemId is not null)
            {
                await navigation.GoToItemAsync(initialItemId);
            }

            await deepLinks.FlushAsync();
        }
    }
}
