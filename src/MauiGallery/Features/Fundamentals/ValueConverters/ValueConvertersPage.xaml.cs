using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class ValueConvertersPage : SamplePage
{
    private readonly PluralConverter _plural = new();

    public ValueConvertersPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        ConvertExample.NativeTarget = RatingLabel;
        PropertiesExample.NativeTarget = OnlineSwitch;
        ParameterExample.NativeTarget = ParameterLabel;
        MultiExample.NativeTarget = MultiButton;
        ParameterNoun.SelectedIndex = 0;
    }

    // ConverterParameter can't be bound, so changing it means creating the binding again.
    private void OnParameterNounChanged(object? sender, EventArgs e) =>
        ParameterLabel.SetBinding(
            Label.TextProperty,
            static (Stepper stepper) => stepper.Value,
            source: ParameterStepper,
            converter: _plural,
            converterParameter: ParameterNoun.SelectedItem as string);

    private void OnCreateAccountClicked(object? sender, EventArgs e) =>
        MultiExample.Output = "Clicked: both conditions are true, so AllTrueConverter returned true";
}
