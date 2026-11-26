using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class TriggersPage : SamplePage
{
    private int _wrongPinCount;

    public TriggersPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        DataExample.NativeTarget = DataButton;
        MultiExample.NativeTarget = MultiButton;
    }

    private void OnSendClicked(object? sender, EventArgs e) =>
        DataExample.Output = $"Sent \"{DataMessage.Text}\"";

    private void OnWrongPinClicked(object? sender, EventArgs e)
    {
        _wrongPinCount++;
        EventExample.Output = $"Clicked: EventTrigger ran ShakeTriggerAction ({_wrongPinCount}x)";
    }

    private void OnShakeDistanceChanged(object? sender, ValueChangedEventArgs e) =>
        EventShake.Distance = Math.Round(e.NewValue);

    private void OnTakeOffClicked(object? sender, EventArgs e) =>
        MultiExample.Output = "Cleared for take-off: both conditions are true";
}
