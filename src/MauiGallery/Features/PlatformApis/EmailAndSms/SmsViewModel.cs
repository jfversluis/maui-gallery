using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the "Compose a text message" sample.</summary>
public sealed partial class SmsViewModel(ISms sms) : ObservableObject
{
    [ObservableProperty]
    public partial string Recipient { get; set; } = "+15555550123";

    [ObservableProperty]
    public partial string Message { get; set; } = "Running late, see you in 10 minutes!";

    [ObservableProperty]
    public partial string? Result { get; set; }

    [RelayCommand]
    private async Task ComposeAsync()
    {
        try
        {
            await sms.ComposeAsync(new SmsMessage(Message, Recipient));
            Result = "Opened the messages app";
        }
        catch (FeatureNotSupportedException)
        {
            // Simulators, Macs and tablets without a phone plan can't send text messages.
            Result = "This device can't send text messages";
        }
    }
}
