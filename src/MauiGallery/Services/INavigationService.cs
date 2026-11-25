namespace MauiGallery.Services;

public interface INavigationService
{
    /// <summary>Opens a gallery item, pushing it onto the current navigation stack.</summary>
    Task GoToItemAsync(string itemId);

    /// <summary>Navigates to a Shell route.</summary>
    Task GoToAsync(string route);
}
