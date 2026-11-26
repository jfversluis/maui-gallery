using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.BasicInput;

public sealed partial class CheckBoxPage : SamplePage
{
    public CheckBoxPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        BasicExample.NativeTarget = BasicCheckBox;
        ColorExample.NativeTarget = ColorCheckBox;
        ListSample.BindingContext = new CheckBoxChecklistViewModel();
        ListExample.NativeTarget = ListSample.GetVisualTreeDescendants().OfType<CheckBox>().FirstOrDefault();
    }

    private void OnBasicCheckedChanged(object? sender, CheckedChangedEventArgs e) =>
        BasicExample.Output = e.Value ? "CheckedChanged: checked" : "CheckedChanged: unchecked";

    private void OnBasicLabelTapped(object? sender, TappedEventArgs e)
    {
        if (BasicCheckBox.IsEnabled)
        {
            BasicCheckBox.IsChecked = !BasicCheckBox.IsChecked;
        }
    }
}
