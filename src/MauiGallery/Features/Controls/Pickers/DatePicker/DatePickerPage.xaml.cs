using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Pickers;

public sealed partial class DatePickerPage : SamplePage
{
    private readonly DatePickerRangeViewModel _range = new();

    public DatePickerPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        RangeSample.BindingContext = _range;
        RangeExample.NativeTarget = RangePicker;
        OptionalSample.BindingContext = new DatePickerOptionalViewModel();
        OptionalExample.NativeTarget = OptionalPicker;
    }

    private void OnBasicDateSelected(object? sender, DateChangedEventArgs e)
    {
        RemeasureBasicPicker();
        BasicExample.Output = $"DateSelected: {e.NewDate:D}";
    }

    private void OnBasicFormatChanged(object? sender, EventArgs e) => RemeasureBasicPicker();

    // DatePicker doesn't invalidate its measure when Format or Date change, so a longer value would be truncated.
    private void RemeasureBasicPicker() => ((IView)BasicDatePicker).InvalidateMeasure();

    private void OnRangeDaysChanged(object? sender, ValueChangedEventArgs e) =>
        _range.DaysAhead = (int)Math.Round(e.NewValue);
}
