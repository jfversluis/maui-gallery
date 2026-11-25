namespace MauiGallery.Services;

/// <summary>Sent through <c>WeakReferenceMessenger</c> when the user's favorites change.</summary>
public sealed record FavoritesChangedMessage(string ItemId, bool IsFavorite);

/// <summary>Sent through <c>WeakReferenceMessenger</c> when an item is added to the recently visited list.</summary>
public sealed record RecentsChangedMessage(string ItemId);
