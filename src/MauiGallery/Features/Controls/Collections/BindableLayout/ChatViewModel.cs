using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Collections;

/// <summary>A message in the "Pick a template per item" BindableLayout sample.</summary>
public sealed record ChatMessage(string Text, bool IsMine);

/// <summary>Chooses the bubble template for a <see cref="ChatMessage"/>: outgoing on the right, incoming on the left.</summary>
public sealed class ChatMessageTemplateSelector : DataTemplateSelector
{
    public DataTemplate? IncomingTemplate { get; set; }

    public DataTemplate? OutgoingTemplate { get; set; }

    protected override DataTemplate? OnSelectTemplate(object item, BindableObject container) =>
        item is ChatMessage { IsMine: true } ? OutgoingTemplate : IncomingTemplate;
}

/// <summary>The view model behind the "Pick a template per item" BindableLayout sample.</summary>
public sealed partial class ChatViewModel : ObservableObject
{
    private const int MaxMessages = 6;

    private static readonly string[] Replies = ["Sounds great!", "Let's do it 🙌", "I'll book the tickets.", "Perfect, see you there."];

    private int _replyIndex;

    public ObservableCollection<ChatMessage> Messages { get; } =
    [
        new("Are we still on for Lisbon next month?", false),
        new("Yes! Flights are booked.", true),
        new("Want to do a food tour on Saturday?", false),
    ];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendCommand))]
    public partial string Draft { get; set; } = string.Empty;

    [RelayCommand(CanExecute = nameof(CanSend))]
    private async Task SendAsync()
    {
        Add(new ChatMessage(Draft.Trim(), true));
        Draft = string.Empty;

        await Task.Delay(800);
        Add(new ChatMessage(Replies[_replyIndex++ % Replies.Length], false));
    }

    private bool CanSend() => !string.IsNullOrWhiteSpace(Draft);

    private void Add(ChatMessage message)
    {
        Messages.Add(message);
        if (Messages.Count > MaxMessages)
        {
            Messages.RemoveAt(0);
        }
    }
}
