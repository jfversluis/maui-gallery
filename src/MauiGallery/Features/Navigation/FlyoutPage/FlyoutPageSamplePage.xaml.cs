using MauiGallery.DesignSystem.Components;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Navigation;

public sealed partial class FlyoutPageSamplePage : SamplePage
{
    public FlyoutPageSamplePage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        LayoutBehavior.SelectedIndexChanged += (_, _) => LayoutBehaviorCode.Value = $"FlyoutLayoutBehavior.{SelectedBehavior}";
        LayoutBehavior.SelectedIndex = 0;
        LayoutHint.Text = DeviceInfo.Idiom == DeviceIdiom.Phone
            ? "On a phone the flyout is always a popover. Try a tablet or desktop to see Split."
            : "Split keeps the menu beside the content. Popover slides it over the content.";
    }

    private FlyoutLayoutBehavior SelectedBehavior =>
        Enum.TryParse<FlyoutLayoutBehavior>(LayoutBehavior.SelectedItem as string, out var behavior) ? behavior : FlyoutLayoutBehavior.Default;

    private async void OnBasicClicked(object? sender, EventArgs e) => await ShowAsync(new FlyoutDemoPage(), BasicExample);

    private async void OnLayoutClicked(object? sender, EventArgs e)
    {
        var page = new FlyoutDemoPage
        {
            FlyoutLayoutBehavior = SelectedBehavior,
            IsGestureEnabled = LayoutGesture.IsToggled,
        };

        // Only ever set true here: false is the default, and a split layout doesn't allow false.
        if (LayoutPresented.IsToggled)
        {
            page.IsPresented = true;
        }
        await ShowAsync(page, LayoutExample);
    }

    private async Task ShowAsync(FlyoutDemoPage page, ControlExample example)
    {
        page.ItemSelected += (_, item) => example.Output = $"Selected {item}: Detail replaced";
        page.IsPresentedChanged += (_, _) => example.Output = $"IsPresentedChanged: {page.IsPresented}";
        page.Disappearing += (_, _) => example.Output = "Closed the flyout page";
        await Navigation.PushModalAsync(page);
        example.Output = "PushModalAsync: showing the flyout page";
    }
}
