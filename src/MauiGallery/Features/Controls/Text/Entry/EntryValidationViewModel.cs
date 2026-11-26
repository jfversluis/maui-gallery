using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Text;

/// <summary>The view model behind the "Validate as you type" Entry sample.</summary>
public sealed partial class EntryValidationViewModel : ObservableObject
{
    public int MaxLength { get; } = 40;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ErrorMessage), nameof(CharacterCount))]
    [NotifyCanExecuteChangedFor(nameof(SubscribeCommand))]
    public partial string Email { get; set; } = string.Empty;

    /// <summary>Feedback for the current input. Empty while the field is empty, so a fresh form shows no errors.</summary>
    public string ErrorMessage => Email.Length == 0 || IsValidEmail ? string.Empty : "Enter an email address like name@example.com";

    public string CharacterCount => $"{Email.Length}/{MaxLength}";

    private bool IsValidEmail => EmailPattern().IsMatch(Email);

    /// <summary>Raised with the address after a successful subscription.</summary>
    public event EventHandler<string>? Subscribed;

    [RelayCommand(CanExecute = nameof(IsValidEmail))]
    private void Subscribe()
    {
        Subscribed?.Invoke(this, Email);
        Email = string.Empty;
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailPattern();
}
