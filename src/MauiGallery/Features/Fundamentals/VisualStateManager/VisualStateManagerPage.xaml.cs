using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class VisualStateManagerPage : SamplePage
{
    public VisualStateManagerPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        CommonExample.NativeTarget = CommonEntry;
        CustomExample.NativeTarget = OrderCard;
        CustomState.SelectedIndex = 0;
        SelectedList.SelectedItem = "Inbox";
        TriggerSample.BindingContext = new EmailValidationViewModel();
    }

    private void OnSelectedChanged(object? sender, SelectionChangedEventArgs e) =>
        SelectedExample.Output = $"Selected: {(e.CurrentSelection.Count > 0 ? e.CurrentSelection[0] : "nothing")}";

    private void OnCustomStateChanged(object? sender, EventArgs e)
    {
        if (CustomState.SelectedItem is string state)
        {
            var moved = VisualStateManager.GoToState(OrderCard, state);
            CustomExample.Output = $"GoToState(orderCard, \"{state}\") returned {moved}";
        }
    }
}
