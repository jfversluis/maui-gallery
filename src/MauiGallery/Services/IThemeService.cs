namespace MauiGallery.Services;

/// <summary>Applies and persists the user's light/dark theme choice.</summary>
public interface IThemeService
{
    ThemePreference Preference { get; set; }

    /// <summary>Applies the saved preference and keeps the theme resources in sync with the active theme.</summary>
    void Initialize(Application application);
}
