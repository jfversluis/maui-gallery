namespace MauiGallery.Features.Controls.Dialogs;

/// <summary>A form that the modal pages sample presents modally and awaits a result from.</summary>
public sealed partial class ContactFormPage : ContentPage
{
    private readonly TaskCompletionSource<Contact?> _result = new();

    public ContactFormPage() => InitializeComponent();

    /// <summary>Completes with the new contact, or <see langword="null"/> when the user cancels.</summary>
    public Task<Contact?> Result => _result.Task;

    // Covers closing without a button: swiping a sheet down on iOS, or Back on Android.
    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _result.TrySetResult(null);
    }

    private void OnTextChanged(object? sender, TextChangedEventArgs e) =>
        SaveButton.IsEnabled = !string.IsNullOrWhiteSpace(NameEntry.Text) && !string.IsNullOrWhiteSpace(EmailEntry.Text);

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        _result.TrySetResult(new Contact(NameEntry.Text.Trim(), EmailEntry.Text.Trim()));
        await Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        _result.TrySetResult(null);
        await Navigation.PopModalAsync();
    }
}
