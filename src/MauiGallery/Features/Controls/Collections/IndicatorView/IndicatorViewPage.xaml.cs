using System.ComponentModel;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Collections;

public sealed partial class IndicatorViewPage : SamplePage
{
    public IndicatorViewPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        BasicExample.NativeTarget = BasicIndicator;
        StepsExample.NativeTarget = StepsIndicator;
        TemplateExample.NativeTarget = TemplateIndicator;
    }

    private void OnStepsBackClicked(object? sender, EventArgs e) => StepsIndicator.Position--;

    private void OnStepsNextClicked(object? sender, EventArgs e) => StepsIndicator.Position++;

    private void OnStepsPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(IndicatorView.Position))
        {
            return;
        }

        var position = StepsIndicator.Position;
        StepsLabel.Text = $"Step {position + 1} of {StepsIndicator.Count}";
        StepsBack.IsEnabled = position > 0;
        StepsNext.IsEnabled = position < StepsIndicator.Count - 1;
        StepsExample.Output = $"Position: {position}";
    }
}
