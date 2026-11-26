using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Text;

public sealed partial class LabelPage : SamplePage
{
    private static readonly Uri LearnMoreUri = new("https://dotnet.microsoft.com/apps/maui");

    public LabelPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        OnMaxLinesChanged(this, EventArgs.Empty);
    }

    private void OnMaxLinesChanged(object? sender, EventArgs e)
    {
        var maxLines = (int)Math.Round(TruncationMaxLines.Value);

        // MaxLines defaults to -1: as many lines as the text needs.
        TruncationLabel.MaxLines = maxLines == 0 ? -1 : maxLines;
        TruncationMaxLinesLabel.Text = maxLines == 0 ? "MaxLines: not set" : $"MaxLines: {maxLines}";
        MaxLinesSubstitution.Value = maxLines == 0 ? null : maxLines;
    }

    private async void OnLearnMoreTapped(object? sender, TappedEventArgs e)
    {
        SpansExample.Output = $"Tapped \"Learn more\", opening {LearnMoreUri}";
        await Browser.Default.OpenAsync(LearnMoreUri, BrowserLaunchMode.SystemPreferred);
    }
}
