using System.ComponentModel;
using System.Globalization;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class TextToSpeechPage : SamplePage
{
    private readonly TextToSpeechViewModel _speech;

    public TextToSpeechPage(SamplePageServices services, TextToSpeechViewModel speech)
        : base(services)
    {
        InitializeComponent();

        _speech = speech;
        SpeakSample.BindingContext = speech;
        VoiceSample.BindingContext = speech;
        VoiceOptions.BindingContext = speech;
        SpeakExample.NativeTarget = SpeakButton;
        VoiceExample.NativeTarget = VoiceButton;
        speech.PropertyChanged += OnSpeechPropertyChanged;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _speech.LoadVoicesCommand.Execute(null);
    }

    protected override void OnDisappearing()
    {
        _speech.StopCommand.Execute(null);
        base.OnDisappearing();
    }

    private static string FloatLiteral(double value) => value.ToString("0.0", CultureInfo.InvariantCulture) + "f";

    private void OnSpeechPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(TextToSpeechViewModel.Status):
                SpeakExample.Output = $"SpeakAsync: {_speech.Status}";
                VoiceExample.Output = SpeakExample.Output;
                break;
            case nameof(TextToSpeechViewModel.Voice):
                LocaleSubstitution.Value = _speech.Voice is { } voice ? $"{voice.Locale.Language}{(string.IsNullOrEmpty(voice.Locale.Country) ? string.Empty : "-" + voice.Locale.Country)}" : null;
                break;
            case nameof(TextToSpeechViewModel.Pitch):
                PitchSubstitution.Value = FloatLiteral(_speech.Pitch);
                break;
            case nameof(TextToSpeechViewModel.Volume):
                VolumeSubstitution.Value = FloatLiteral(_speech.Volume);
                break;
            case nameof(TextToSpeechViewModel.Rate):
                RateSubstitution.Value = FloatLiteral(_speech.Rate);
                break;
        }
    }
}
