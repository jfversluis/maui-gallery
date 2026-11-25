namespace MauiGallery.Services;

/// <summary>Sent through <c>WeakReferenceMessenger</c> when the user's favorites change.</summary>
public sealed record FavoritesChangedMessage(string ItemId, bool IsFavorite);

/// <summary>Sent through <c>WeakReferenceMessenger</c> when an item is added to the recently visited list.</summary>
public sealed record RecentsChangedMessage(string ItemId);

/// <summary>Sent through <c>WeakReferenceMessenger</c> when the user picks a different theme.</summary>
public sealed record ThemePreferenceChangedMessage(ThemePreference Preference);
