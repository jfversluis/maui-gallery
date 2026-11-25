using CommunityToolkit.Mvvm.Messaging;
using MauiGallery.DesignSystem.Tokens;

namespace MauiGallery.Services;

/// <summary>
/// Maps the theme preference onto <see cref="Application.UserAppTheme"/>, which also switches the native controls,
/// and swaps the design system's light and dark token dictionaries to match.
/// </summary>
public sealed class ThemeService(IPreferences preferences, IMessenger messenger) : IThemeService
{
    private const string PreferenceKey = "theme";

    private Application? _application;

    public ThemePreference Preference
    {
        get => Enum.TryParse<ThemePreference>(preferences.Get(PreferenceKey, nameof(ThemePreference.System)), out var value)
            ? value
            : ThemePreference.System;
        set
        {
            if (value == Preference)
            {
                return;
            }

            preferences.Set(PreferenceKey, value.ToString());
            Apply(value);
            messenger.Send(new ThemePreferenceChangedMessage(value));
        }
    }

    public void Initialize(Application application)
    {
        ArgumentNullException.ThrowIfNull(application);

        _application = application;
        application.RequestedThemeChanged += (_, e) => ApplyTokens(e.RequestedTheme);
        Apply(Preference);
        ApplyTokens(application.RequestedTheme);
    }

    private void Apply(ThemePreference preference)
    {
        if (_application is null)
        {
            return;
        }

        _application.UserAppTheme = preference switch
        {
            ThemePreference.Light => AppTheme.Light,
            ThemePreference.Dark => AppTheme.Dark,
            _ => AppTheme.Unspecified,
        };
    }

    private void ApplyTokens(AppTheme theme)
    {
        var merged = _application?.Resources.MergedDictionaries;
        if (merged is null)
        {
            return;
        }

        var wantDark = theme == AppTheme.Dark;
        var current = merged.FirstOrDefault(d => d is LightTheme or DarkTheme);
        if (current is not null && current is DarkTheme == wantDark)
        {
            return;
        }

        // Add the new tokens before removing the old ones, so no lookup ever sees a missing key.
        merged.Add(wantDark ? new DarkTheme() : new LightTheme());
        if (current is not null)
        {
            merged.Remove(current);
        }
    }
}
