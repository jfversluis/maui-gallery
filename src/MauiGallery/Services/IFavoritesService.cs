namespace MauiGallery.Services;

public interface IFavoritesService
{
    IReadOnlyList<string> Ids { get; }

    bool IsFavorite(string itemId);

    /// <summary>Adds or removes the item and returns whether it is now a favorite.</summary>
    bool Toggle(string itemId);
}
