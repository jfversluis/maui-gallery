using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Dialogs;

public sealed partial class PromptsPage : SamplePage
{
    public PromptsPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        InputKeyboard.SelectedIndex = 1;
        RenameExample.NativeTarget = RenameButton;
    }

    private async void OnBasicClicked(object? sender, EventArgs e)
    {
        var name = await DisplayPromptAsync(BasicTitle.Text, BasicMessage.Text, accept: BasicAccept.Text);
        BasicExample.Output = Describe(name);
    }

    private async void OnInputClicked(object? sender, EventArgs e)
    {
        var code = await DisplayPromptAsync(
            "Verify your email",
            "Enter the code we sent to you.",
            placeholder: InputPlaceholder.Text,
            maxLength: (int)InputMaxLength.Value,
            keyboard: SelectedKeyboard,
            initialValue: InputInitialValue.Text ?? string.Empty,
            accept: "Verify");
        InputExample.Output = Describe(code);
    }

    private async void OnRenameClicked(object? sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("Rename", "Enter a new name for this album.", initialValue: RenameName.Text);
        if (name is null)
        {
            RenameExample.Output = "Cancelled: the name didn't change";
        }
        else if (string.IsNullOrWhiteSpace(name))
        {
            RenameExample.Output = "Empty answer ignored";
        }
        else
        {
            RenameName.Text = name.Trim();
            RenameExample.Output = $"Renamed to \"{RenameName.Text}\"";
        }
    }

    private Keyboard SelectedKeyboard => (string?)InputKeyboard.SelectedItem switch
    {
        "Numeric" => Keyboard.Numeric,
        "Telephone" => Keyboard.Telephone,
        "Email" => Keyboard.Email,
        "Url" => Keyboard.Url,
        "Text" => Keyboard.Text,
        "Chat" => Keyboard.Chat,
        "Plain" => Keyboard.Plain,
        _ => Keyboard.Default,
    };

    private void OnInputKeyboardChanged(object? sender, EventArgs e) =>
        InputKeyboardSubstitution.Value = $"Keyboard.{InputKeyboard.SelectedItem}";

    private static string Describe(string? result) =>
        result is null ? "DisplayPromptAsync returned null (cancelled)" : $"DisplayPromptAsync returned \"{result}\"";
}
