using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Text;

public sealed partial class EntryPage : SamplePage
{
    public EntryPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        var validation = new EntryValidationViewModel();
        validation.Subscribed += (_, email) => ValidationExample.Output = $"Subscribed {email}";
        ValidationSample.BindingContext = validation;
        ValidationExample.NativeTarget = ValidationEntry;
        PasswordExample.NativeTarget = PasswordEntry;

        OnKeyboardTypeChanged(this, EventArgs.Empty);
    }

    private void OnBasicTextChanged(object? sender, TextChangedEventArgs e) =>
        BasicExample.Output = $"TextChanged: \"{e.NewTextValue}\"";

    private void OnKeyboardTypeChanged(object? sender, EventArgs e) =>
        KeyboardEntry.Keyboard = KeyboardType.SelectedItem switch
        {
            "Text" => Keyboard.Text,
            "Email" => Keyboard.Email,
            "Url" => Keyboard.Url,
            "Numeric" => Keyboard.Numeric,
            "Telephone" => Keyboard.Telephone,
            "Chat" => Keyboard.Chat,
            _ => Keyboard.Default,
        };

    private void OnKeyboardCompleted(object? sender, EventArgs e) =>
        KeyboardExample.Output = $"Completed: \"{KeyboardEntry.Text}\"";

    private void OnShowPasswordChanged(object? sender, CheckedChangedEventArgs e) =>
        PasswordEntry.IsPassword = !e.Value;
}
