using CommunityToolkit.Mvvm.Messaging;
using MauiGallery.Core.Collections;

namespace MauiGallery.Services;

public sealed class FavoritesService(IPreferences preferences, IMessenger messenger) : IFavoritesService
{
    private const string PreferenceKey = "favorites";

    private readonly List<string> _ids = [.. IdListSerializer.Deserialize(preferences.Get(PreferenceKey, string.Empty))];

    public IReadOnlyList<string> Ids => _ids;

    public bool IsFavorite(string itemId) => _ids.Contains(itemId);

    public bool Toggle(string itemId)
    {
        var isFavorite = !_ids.Remove(itemId);
        if (isFavorite)
        {
            _ids.Insert(0, itemId);
        }

        preferences.Set(PreferenceKey, IdListSerializer.Serialize(_ids));
        messenger.Send(new FavoritesChangedMessage(itemId, isFavorite));
        return isFavorite;
    }
}
