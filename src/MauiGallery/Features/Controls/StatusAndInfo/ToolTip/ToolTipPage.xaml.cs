using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.StatusAndInfo;

public sealed partial class ToolTipPage : SamplePage
{
    public ToolTipPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        ToolbarExample.NativeTarget = BoldButton;
        AnyViewExample.NativeTarget = AutoSaveSwitch;
    }

    private void OnBasicClicked(object? sender, EventArgs e) => BasicExample.Output = "Clicked";

    private void OnToolbarClicked(object? sender, EventArgs e)
    {
        if (sender is ImageButton button)
        {
            ToolbarExample.Output = $"Clicked: {SemanticProperties.GetDescription(button)}";
        }
    }
}
