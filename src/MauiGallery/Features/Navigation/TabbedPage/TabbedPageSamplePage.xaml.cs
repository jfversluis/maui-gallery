using MauiGallery.DesignSystem.Components;
using MauiGallery.Features.Samples;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using AndroidPlatform = Microsoft.Maui.Controls.PlatformConfiguration.Android;

namespace MauiGallery.Features.Navigation;

public sealed partial class TabbedPageSamplePage : SamplePage
{
    public TabbedPageSamplePage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        PlacementPicker.SelectedIndex = 0;
        PlacementPicker.SelectedIndexChanged += (_, _) => UpdatePlacementCode();
        UpdatePlacementCode();
        PlacementHint.Text = DeviceInfo.Platform == DevicePlatform.Android
            ? "Open it with each placement to compare."
            : $"Only Android reads these settings. On {DeviceInfo.Platform}, the tabs stay where the platform puts them.";
    }

    private async void OnBasicClicked(object? sender, EventArgs e)
    {
        var page = new TabbedDemoPage();
        if (BasicBarColor.SelectedColor is { } bar)
        {
            page.BarBackgroundColor = bar;
        }

        if (BasicSelectedColor.SelectedColor is { } selected)
        {
            page.SelectedTabColor = selected;
        }

        if (BasicUnselectedColor.SelectedColor is { } unselected)
        {
            page.UnselectedTabColor = unselected;
        }

        await ShowAsync(page, BasicExample);
    }

    private void OnBasicResetClicked(object? sender, EventArgs e)
    {
        BasicBarColor.SelectedColor = null;
        BasicSelectedColor.SelectedColor = null;
        BasicUnselectedColor.SelectedColor = null;
    }

    private async void OnBadgeClicked(object? sender, EventArgs e)
    {
        var page = new TabbedDemoPage();
        page.ShowInboxBadge(BadgeText.Text ?? string.Empty, BadgeColor.SelectedColor);
        await ShowAsync(page, BadgeExample);
    }

    private async void OnPlacementClicked(object? sender, EventArgs e)
    {
        var page = new TabbedDemoPage();
        page.On<AndroidPlatform>()
            .SetToolbarPlacement(SelectedPlacement)
            .SetIsSwipePagingEnabled(PlacementSwipe.IsToggled);
        await ShowAsync(page, PlacementExample);
    }

    private ToolbarPlacement SelectedPlacement =>
        Enum.TryParse<ToolbarPlacement>(PlacementPicker.SelectedItem as string, out var placement) ? placement : ToolbarPlacement.Default;

    private void UpdatePlacementCode() =>
        PlacementCodeSubstitution.Value = $"ToolbarPlacement.{SelectedPlacement}";

    private async Task ShowAsync(TabbedDemoPage page, ControlExample example)
    {
        page.CurrentPageChanged += (_, _) => example.Output = $"CurrentPageChanged: {page.CurrentPage?.Title}";
        page.Disappearing += (_, _) => example.Output = "Closed the tabbed page";
        await Navigation.PushModalAsync(page);
        example.Output = $"PushModalAsync: showing {page.Children.Count} tabs";
    }
}
