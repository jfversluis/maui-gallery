using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the Preferences sample. Every change is saved straight away.</summary>
public sealed partial class SettingsStoreViewModel : ObservableObject
{
    // A container of its own, so Clear never touches the gallery's own settings (theme, favorites, recents).
    public const string SharedName = "gallery.preferences-sample";

    private const string NameKey = "user_name";
    private const string NotificationsKey = "notifications";
    private const string TextSizeKey = "text_size";
    private const string LastSavedKey = "last_saved";

    private readonly IPreferences _preferences;
    private bool _isLoading;

    public SettingsStoreViewModel(IPreferences preferences)
    {
        _preferences = preferences;
        Load();
    }

    [ObservableProperty]
    public partial string UserName { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool Notifications { get; set; }

    [ObservableProperty]
    public partial double TextSize { get; set; }

    /// <summary>What is in the preferences container right now, one line per key.</summary>
    [ObservableProperty]
    public partial IReadOnlyList<string> Stored { get; set; } = [];

    partial void OnUserNameChanged(string value) => Save(() => _preferences.Set(NameKey, value, SharedName));

    partial void OnNotificationsChanged(bool value) => Save(() => _preferences.Set(NotificationsKey, value, SharedName));

    partial void OnTextSizeChanged(double value) => Save(() => _preferences.Set(TextSizeKey, (int)value, SharedName));

    [RelayCommand]
    private void RemoveName()
    {
        _preferences.Remove(NameKey, SharedName);
        Load();
    }

    [RelayCommand]
    private void ClearAll()
    {
        _preferences.Clear(SharedName);
        Load();
    }

    private void Save(Action set)
    {
        if (_isLoading)
        {
            return;
        }

        set();
        _preferences.Set(LastSavedKey, DateTime.Now, SharedName);
        ReadStored();
    }

    private void Load()
    {
        _isLoading = true;
        UserName = _preferences.Get(NameKey, string.Empty, SharedName);
        Notifications = _preferences.Get(NotificationsKey, true, SharedName);
        TextSize = _preferences.Get(TextSizeKey, 16, SharedName);
        _isLoading = false;
        ReadStored();
    }

    private void ReadStored()
    {
        var stored = new List<string>();
        if (_preferences.ContainsKey(NameKey, SharedName))
        {
            stored.Add($"{NameKey} = \"{_preferences.Get(NameKey, string.Empty, SharedName)}\"");
        }

        if (_preferences.ContainsKey(NotificationsKey, SharedName))
        {
            stored.Add($"{NotificationsKey} = {_preferences.Get(NotificationsKey, false, SharedName)}");
        }

        if (_preferences.ContainsKey(TextSizeKey, SharedName))
        {
            stored.Add($"{TextSizeKey} = {_preferences.Get(TextSizeKey, 0, SharedName)}");
        }

        if (_preferences.ContainsKey(LastSavedKey, SharedName))
        {
            stored.Add($"{LastSavedKey} = {_preferences.Get(LastSavedKey, DateTime.MinValue, SharedName):T}");
        }

        Stored = stored.Count > 0 ? stored : ["Nothing stored"];
    }
}
