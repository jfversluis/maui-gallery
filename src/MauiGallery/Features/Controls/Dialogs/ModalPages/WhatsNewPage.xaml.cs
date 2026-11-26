namespace MauiGallery.Features.Controls.Dialogs;

/// <summary>A simple page that the modal pages sample presents modally.</summary>
public sealed partial class WhatsNewPage : ContentPage
{
    public WhatsNewPage() => InitializeComponent();

    private async void OnCloseClicked(object? sender, EventArgs e) => await Navigation.PopModalAsync();
}
