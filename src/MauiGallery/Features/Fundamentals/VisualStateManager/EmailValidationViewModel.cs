using System.Net.Mail;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.Fundamentals;

/// <summary>Exposes one flag per visual state, so state triggers can pick the state from the data.</summary>
public sealed partial class EmailValidationViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEmpty), nameof(IsValid), nameof(IsInvalid))]
    public partial string? Email { get; set; }

    public bool IsEmpty => string.IsNullOrWhiteSpace(Email);

    public bool IsValid => !IsEmpty && MailAddress.TryCreate(Email, out _);

    public bool IsInvalid => !IsEmpty && !IsValid;
}
