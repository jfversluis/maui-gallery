using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Text;

public sealed partial class EditorPage : SamplePage
{
    public EditorPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        LimitExample.NativeTarget = LimitEditor;
        UpdateLimitCounter();
    }

    private void OnBasicTextChanged(object? sender, TextChangedEventArgs e) =>
        BasicExample.Output = $"TextChanged: {e.NewTextValue?.Length ?? 0} characters";

    private void OnBasicCompleted(object? sender, EventArgs e) =>
        BasicExample.Output = "Completed: the editor lost focus";

    private void OnLimitTextChanged(object? sender, TextChangedEventArgs e) => UpdateLimitCounter();

    private void OnLimitMaxLengthChanged(object? sender, ValueChangedEventArgs e) => UpdateLimitCounter();

    private void UpdateLimitCounter() =>
        LimitCounter.Text = $"{LimitEditor.Text?.Length ?? 0}/{LimitEditor.MaxLength}";
}
