namespace MauiGallery.Services;

/// <summary>Opens additional app windows on platforms that support them (desktop and iPad).</summary>
public interface IWindowService
{
    bool CanOpenWindows { get; }

    void OpenNew();

    /// <summary>Opens a new window that shows the given gallery item.</summary>
    void OpenItem(string itemId);
}
