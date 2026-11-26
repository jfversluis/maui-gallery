using MauiGallery.DesignSystem.Components;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Navigation;

public sealed partial class NavigationPageSamplePage : SamplePage
{
    public NavigationPageSamplePage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        StackTextColor.SelectedIndexChanged += (_, _) => UpdateTextColorCode();
        StackTextColor.SelectedIndex = 0;
    }

    private Color? SelectedTextColor => (StackTextColor.SelectedItem as string) switch
    {
        "White" => Colors.White,
        "Black" => Colors.Black,
        _ => null,
    };

    private void UpdateTextColorCode()
    {
        StackTextCodeSubstitution.Value = SelectedTextColor is null ? null : $"Colors.{StackTextColor.SelectedItem}";
    }

    private void OnStackResetClicked(object? sender, EventArgs e)
    {
        StackBarColor.SelectedColor = null;
        StackTextColor.SelectedIndex = 0;
    }

    private async void OnStackClicked(object? sender, EventArgs e)
    {
        var navigation = new NavigationPage(new NavigationDemoPage(1, NavigationDemoOptions.Default));
        if (StackBarColor.SelectedColor is { } bar)
        {
            navigation.BarBackgroundColor = bar;
        }

        if (SelectedTextColor is { } text)
        {
            navigation.BarTextColor = text;
        }

        await ShowAsync(navigation, StackExample);
    }

    private async void OnBarClicked(object? sender, EventArgs e)
    {
        var options = new NavigationDemoOptions(
            CustomTitleView: true,
            BackButtonTitle: string.IsNullOrWhiteSpace(BarBackTitle.Text) ? null : BarBackTitle.Text,
            HasNavigationBar: BarNavigationBar.IsToggled,
            HasBackButton: BarBackButton.IsToggled);
        await ShowAsync(new NavigationPage(new NavigationDemoPage(1, options)), BarExample);
    }

    private async Task ShowAsync(NavigationPage navigation, ControlExample example)
    {
        navigation.Pushed += (_, e) => example.Output = $"Pushed {e.Page.Title}: {navigation.Navigation.NavigationStack.Count} pages";
        navigation.Popped += (_, e) => example.Output = $"Popped {e.Page.Title}: {navigation.Navigation.NavigationStack.Count} pages";
        navigation.PoppedToRoot += (_, _) => example.Output = "PoppedToRoot: 1 page";
        await Navigation.PushModalAsync(navigation);
        example.Output = "PushModalAsync: showing the navigation page";
    }
}
