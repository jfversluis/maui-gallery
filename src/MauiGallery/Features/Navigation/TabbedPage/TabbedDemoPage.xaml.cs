namespace MauiGallery.Features.Navigation;

/// <summary>A three-tab page that the tabbed page sample presents modally.</summary>
public sealed partial class TabbedDemoPage : TabbedPage
{
    public TabbedDemoPage() => InitializeComponent();

    /// <summary>Shows a badge on the Inbox tab, with a button on that tab that clears it.</summary>
    public void ShowInboxBadge(string text, Color? color)
    {
        SetBadgeText(InboxPage, text);
        SetBadgeColor(InboxPage, color);
        MarkReadButton.IsVisible = true;
    }

    private void OnMarkReadClicked(object? sender, EventArgs e)
    {
        SetBadgeText(InboxPage, null);
        MarkReadButton.IsEnabled = false;
    }

    private void OnGoToSettingsClicked(object? sender, EventArgs e) => CurrentPage = SettingsPage;

    private async void OnCloseClicked(object? sender, EventArgs e) => await Navigation.PopModalAsync();
}
