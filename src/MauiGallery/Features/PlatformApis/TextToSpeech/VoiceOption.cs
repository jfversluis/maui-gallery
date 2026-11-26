namespace MauiGallery.Features.PlatformApis;

/// <summary>A text-to-speech voice, with a name to show in a picker.</summary>
public sealed record VoiceOption(Locale Locale)
{
    public string DisplayName => string.IsNullOrEmpty(Locale.Country)
        ? $"{Locale.Name} ({Locale.Language})"
        : $"{Locale.Name} ({Locale.Language}-{Locale.Country})";
}
