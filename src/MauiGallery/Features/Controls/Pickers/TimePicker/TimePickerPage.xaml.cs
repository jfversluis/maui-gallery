using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Pickers;

public sealed partial class TimePickerPage : SamplePage
{
    public TimePickerPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        OptionalSample.BindingContext = new TimePickerOptionalViewModel();
        OptionalExample.NativeTarget = OptionalPicker;
    }

    private void OnBasicTimeSelected(object? sender, TimeChangedEventArgs e)
    {
        RemeasureBasicPicker();
        BasicExample.Output = e.NewTime is { } time ? $"TimeSelected: {DateTime.Today.Add(time):T}" : "TimeSelected: no time";
    }

    private void OnBasicFormatChanged(object? sender, EventArgs e) => RemeasureBasicPicker();

    // TimePicker doesn't invalidate its measure when Format or Time change, so a longer value would be truncated.
    private void RemeasureBasicPicker() => ((IView)BasicTimePicker).InvalidateMeasure();
}
