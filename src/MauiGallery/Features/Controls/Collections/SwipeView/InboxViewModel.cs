using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Collections;

/// <summary>A message in the "Swipe to reveal actions" SwipeView sample.</summary>
public sealed partial class InboxMessage(string sender, string subject, string time) : ObservableObject
{
    public string Sender { get; } = sender;

    public string Subject { get; } = subject;

    public string Time { get; } = time;

    [ObservableProperty]
    public partial bool IsFlagged { get; set; }
}

/// <summary>The view model behind the "Swipe to reveal actions" SwipeView sample.</summary>
public sealed partial class InboxViewModel : ObservableObject
{
    public InboxViewModel() => Restore();

    public ObservableCollection<InboxMessage> Messages { get; } = [];

    public event EventHandler<string>? ActionInvoked;

    [RelayCommand]
    private void Archive(InboxMessage message)
    {
        Messages.Remove(message);
        ActionInvoked?.Invoke(this, $"Archive invoked: {message.Sender}");
    }

    [RelayCommand]
    private void Delete(InboxMessage message)
    {
        Messages.Remove(message);
        ActionInvoked?.Invoke(this, $"Delete invoked: {message.Sender}");
    }

    [RelayCommand]
    private void ToggleFlag(InboxMessage message)
    {
        message.IsFlagged = !message.IsFlagged;
        ActionInvoked?.Invoke(this, $"Flag invoked: {message.Sender} is {(message.IsFlagged ? "flagged" : "unflagged")}");
    }

    [RelayCommand]
    private void Restore()
    {
        Messages.Clear();
        Messages.Add(new("Ana Costa", "Lisbon itinerary, final version", "9:41"));
        Messages.Add(new("Kenji Sato", "Ryokan booking confirmed", "8:15"));
        Messages.Add(new("Travel Desk", "Your boarding pass is ready", "Yesterday"));
        Messages.Add(new("Lena Fischer", "Photos from the hike", "Mon"));
    }
}
