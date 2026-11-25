using CommunityToolkit.Mvvm.Messaging;
using MauiGallery.Core.Collections;

namespace MauiGallery.Services;

public sealed class RecentsService(IPreferences preferences, IMessenger messenger) : IRecentsService
{
    private const string PreferenceKey = "recents";
    private const int Capacity = 8;

    private readonly RecentItems _recents = new(Capacity, IdListSerializer.Deserialize(preferences.Get(PreferenceKey, string.Empty)));

    public IReadOnlyList<string> Ids => _recents.Ids;

    public void Add(string itemId)
    {
        if (_recents.Add(itemId))
        {
            preferences.Set(PreferenceKey, IdListSerializer.Serialize(_recents.Ids));
            messenger.Send(new RecentsChangedMessage(itemId));
        }
    }
}
