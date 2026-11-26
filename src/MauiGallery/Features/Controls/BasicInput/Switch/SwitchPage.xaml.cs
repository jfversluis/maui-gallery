using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.BasicInput;

public sealed partial class SwitchPage : SamplePage
{
    public SwitchPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        BasicExample.NativeTarget = BasicSwitch;
        ColorExample.NativeTarget = ColorOnSwitch;
        SettingsSample.BindingContext = new SwitchSettingsViewModel();
        SettingsExample.NativeTarget = NotificationsSwitch;
    }

    private void OnBasicToggled(object? sender, ToggledEventArgs e) =>
        BasicExample.Output = e.Value ? "Toggled: on" : "Toggled: off";
}
