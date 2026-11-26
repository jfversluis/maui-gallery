using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.BasicInput;

public sealed partial class RadioButtonPage : SamplePage
{
    public RadioButtonPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        BasicExample.NativeTarget = StandardRadio;
        GroupSample.BindingContext = new RadioButtonSizeViewModel();
        GroupExample.NativeTarget = SmallRadio;
        ContentExample.NativeTarget = LightThemeRadio;
        TemplateExample.NativeTarget = CarRadio;
    }

    private void OnBasicCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (e.Value && sender is RadioButton radioButton)
        {
            BasicExample.Output = $"CheckedChanged: {radioButton.Content}";
        }
    }

    private void OnTemplateCheckedChanged(object? sender, CheckedChangedEventArgs e)
    {
        if (e.Value && sender is RadioButton radioButton)
        {
            TemplateExample.Output = $"Selected value: {radioButton.Value}";
        }
    }
}
