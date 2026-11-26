using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the Text-to-speech samples.</summary>
public sealed partial class TextToSpeechViewModel(ITextToSpeech textToSpeech) : ObservableObject
{
    [ObservableProperty]
    public partial string Text { get; set; } = "Hello! I'm reading this text with the device's own voice.";

    [ObservableProperty]
    public partial IReadOnlyList<VoiceOption> Voices { get; set; } = [];

    [ObservableProperty]
    public partial VoiceOption? Voice { get; set; }

    [ObservableProperty]
    public partial double Pitch { get; set; } = 1.0;

    [ObservableProperty]
    public partial double Volume { get; set; } = 1.0;

    [ObservableProperty]
    public partial double Rate { get; set; } = 1.0;

    [ObservableProperty]
    public partial string Status { get; set; } = "Ready";

    // Compiled bindings can't see the generated SpeakCancelCommand; expose it through a declared property.
    public ICommand StopCommand => SpeakCancelCommand;

    /// <summary>Loads the voices the device has. Call it when the page appears.</summary>
    [RelayCommand]
    private async Task LoadVoicesAsync()
    {
        if (Voices.Count > 0)
        {
            return;
        }

        var locales = await textToSpeech.GetLocalesAsync();
        Voices = [.. locales.Select(l => new VoiceOption(l)).OrderBy(v => v.DisplayName, StringComparer.CurrentCulture)];

        // Start with a voice for the person's own language, when there is one.
        var language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        Voice = Voices.FirstOrDefault(v => string.Equals(v.Locale.Language, language, StringComparison.OrdinalIgnoreCase)) ?? (Voices.Count > 0 ? Voices[0] : null);
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task SpeakAsync(CancellationToken cancellationToken)
    {
        Status = "Speaking…";
        try
        {
            await textToSpeech.SpeakAsync(Text, new SpeechOptions
            {
                Locale = Voice?.Locale,
                Pitch = (float)Pitch,
                Volume = (float)Volume,
                Rate = (float)Rate,
            }, cancellationToken);
            Status = cancellationToken.IsCancellationRequested ? "Stopped" : "Finished";
        }
        catch (OperationCanceledException)
        {
            Status = "Stopped";
        }
    }
}
