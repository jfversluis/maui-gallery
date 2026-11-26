using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.Controls.BasicInput;

/// <summary>The view model behind the "Bind to settings" Switch sample.</summary>
public sealed partial class SwitchSettingsViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    public partial bool NotificationsEnabled { get; set; } = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    public partial bool SoundEnabled { get; set; } = true;

    public string Summary => (NotificationsEnabled, SoundEnabled) switch
    {
        (false, _) => "You won't be notified",
        (true, true) => "Notifications with sound",
        (true, false) => "Silent notifications",
    };
}
