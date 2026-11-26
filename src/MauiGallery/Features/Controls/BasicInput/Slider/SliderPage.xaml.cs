using System.Globalization;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.BasicInput;

public sealed partial class SliderPage : SamplePage
{
    private double _step = 4;

    public SliderPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        BasicExample.NativeTarget = BasicSlider;
        StepExample.NativeTarget = StepSlider;
        StepSize.SelectedIndex = 2;
    }

    private void OnBasicValueChanged(object? sender, ValueChangedEventArgs e) =>
        BasicExample.Output = $"ValueChanged: {e.OldValue:0.#} → {e.NewValue:0.#}";

    private void OnStepValueChanged(object? sender, ValueChangedEventArgs e)
    {
        // Setting Value raises ValueChanged again with the snapped value, which is then left alone.
        StepSlider.Value = Math.Round(e.NewValue / _step) * _step;
    }

    private void OnStepDragStarted(object? sender, EventArgs e) => StepExample.Output = "DragStarted";

    private void OnStepDragCompleted(object? sender, EventArgs e) =>
        StepExample.Output = $"DragCompleted at {StepSlider.Value:0}";

    private void OnStepSizeChanged(object? sender, EventArgs e)
    {
        _step = double.Parse((string)StepSize.SelectedItem, CultureInfo.InvariantCulture);
        StepSubstitution.Value = _step;
        StepSlider.Value = Math.Round(StepSlider.Value / _step) * _step;
    }
}
