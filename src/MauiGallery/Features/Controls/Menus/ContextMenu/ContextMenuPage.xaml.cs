using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Menus;

public sealed partial class ContextMenuPage : SamplePage
{
    public ContextMenuPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        var files = new FilesViewModel();
        files.ActionInvoked += (_, message) => ListExample.Output = message;
        ListSample.BindingContext = files;
        ListRestore.Command = files.RestoreCommand;
    }

    private void OnBasicItemClicked(object? sender, EventArgs e) =>
        BasicExample.Output = $"Clicked: {((MenuFlyoutItem)sender!).Text}";

    private async void OnTileLongPressed(object? sender, LongPressedEventArgs e)
    {
        var action = await DisplayActionSheetAsync("Beach.jpg", "Cancel", "Delete", "Share", "Duplicate");
        LongPressExample.Output = action is null ? "Action sheet dismissed" : $"Chose: {action}";
    }
}
