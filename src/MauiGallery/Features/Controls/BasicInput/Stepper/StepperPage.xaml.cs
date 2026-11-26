using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.BasicInput;

public sealed partial class StepperPage : SamplePage
{
    public StepperPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        BasicExample.NativeTarget = BasicStepper;
        QuantitySample.BindingContext = new StepperTicketsViewModel();
        QuantityExample.NativeTarget = QuantityStepper;
        BasicIncrement.SelectedIndex = 0;
    }

    private void OnBasicValueChanged(object? sender, ValueChangedEventArgs e) =>
        BasicExample.Output = $"ValueChanged: {e.OldValue} → {e.NewValue}";

    private void OnBasicIncrementChanged(object? sender, EventArgs e)
    {
        var increment = (double)BasicIncrement.SelectedItem;
        BasicStepper.Increment = increment;
        IncrementSubstitution.Value = increment;
    }
}
