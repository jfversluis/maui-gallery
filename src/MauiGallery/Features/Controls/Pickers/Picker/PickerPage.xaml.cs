using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Pickers;

public sealed partial class PickerPage : SamplePage
{
    public PickerPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        BindingSample.BindingContext = new PickerBindingViewModel();
        BasicExample.NativeTarget = BasicPicker;
        BindingExample.NativeTarget = BindingPicker;
    }

    private void OnBasicSelectedIndexChanged(object? sender, EventArgs e) =>
        BasicExample.Output = BasicPicker.SelectedIndex < 0
            ? "SelectedIndexChanged: nothing selected (-1)"
            : $"SelectedIndexChanged: {BasicPicker.SelectedIndex} ({BasicPicker.SelectedItem})";

    private void OnBasicClearClicked(object? sender, EventArgs e) => BasicPicker.SelectedIndex = -1;
}
