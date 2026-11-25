using Microsoft.Extensions.Logging;

namespace MauiGallery.Services;

/// <summary>Shell-based navigation that always targets the window the user is interacting with.</summary>
public sealed partial class ShellNavigationService(ILogger<ShellNavigationService> logger) : INavigationService
{
    public Task GoToItemAsync(string itemId) => GoToAsync(itemId);

    public async Task GoToAsync(string route)
    {
        if (Shell.Current is not { } shell)
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
