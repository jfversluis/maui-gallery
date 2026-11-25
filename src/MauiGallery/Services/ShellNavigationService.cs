using Microsoft.Extensions.Logging;

namespace MauiGallery.Services;

/// <summary>Shell navigation that targets the window the user is working in.</summary>
public sealed partial class ShellNavigationService(ActiveWindowTracker windows, ILogger<ShellNavigationService> logger) : INavigationService
{
    /// <summary>
    /// With the sidebar, items always open in the Controls section so the sidebar selection and back stack stay
    /// consistent. With bottom tabs, items push onto the current tab, as people expect on a phone.
    /// </summary>
    public Task GoToItemAsync(string itemId) =>
        windows.ActiveShell is { FlyoutBehavior: not FlyoutBehavior.Disabled }
            ? GoToAsync($"{Features.Catalog.Routes.AllControls}/{itemId}")
            : GoToAsync(itemId);

    public async Task GoToAsync(string route)
    {
        if (windows.ActiveShell is not { } shell)
        {
            return;
        }

        try
        {
            await shell.GoToAsync(route);
        }
        catch (Exception ex)
        {
            // A page that fails to load must not take the app down. Log it so DevFlow and the route crawler see it.
            LogNavigationFailed(logger, ex, route);
        }
    }

    [LoggerMessage(Level = LogLevel.Error, Message = "Navigation to '{Route}' failed")]
    private static partial void LogNavigationFailed(ILogger logger, Exception exception, string route);
}
