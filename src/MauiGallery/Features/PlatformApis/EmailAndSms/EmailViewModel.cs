using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the "Compose an email" sample.</summary>
public sealed partial class EmailViewModel(IEmail email, IFileSystem fileSystem) : ObservableObject
{
    [ObservableProperty]
    public partial string To { get; set; } = "support@example.com";

    [ObservableProperty]
    public partial string Subject { get; set; } = "Feedback on the gallery";

    [ObservableProperty]
    public partial string Body { get; set; } = "Hi! I'd love to see a sample for…";

    [ObservableProperty]
    public partial bool IsHtml { get; set; }

    [ObservableProperty]
    public partial bool AttachLog { get; set; }

    [ObservableProperty]
    public partial string? Result { get; set; }

    [RelayCommand]
    private async Task ComposeAsync()
    {
        // Simulators and computers often have no mail account set up.
        if (!email.IsComposeSupported)
        {
            Result = "No mail app is set up on this device";
            return;
        }

        var message = new EmailMessage
        {
            Subject = Subject,
            Body = IsHtml ? $"<p>{System.Net.WebUtility.HtmlEncode(Body)}</p><p><b>Sent from the .NET MAUI Gallery</b></p>" : Body,
            BodyFormat = IsHtml ? EmailBodyFormat.Html : EmailBodyFormat.PlainText,
            To = [.. To.Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)],
        };

        if (AttachLog)
        {
            var path = Path.Combine(fileSystem.CacheDirectory, "gallery-log.txt");
            await File.WriteAllTextAsync(path, $"Log written at {DateTime.Now:G}");
            message.Attachments?.Add(new EmailAttachment(path));
        }

        try
        {
            await email.ComposeAsync(message);
            Result = "Opened the mail composer";
        }
        catch (FeatureNotSupportedException)
        {
            Result = "This device can't compose email";
        }
    }
}
