using IconFont.Maui.FluentIcons;

namespace MauiGallery.Features.Navigation;

/// <summary>A numbered page in the navigation page sample's stack.</summary>
public sealed partial class NavigationDemoPage : ContentPage
{
    private readonly int _level;
    private readonly NavigationDemoOptions _options;

    public NavigationDemoPage(int level, NavigationDemoOptions options)
    {
        InitializeComponent();
        _level = level;
        _options = options;

        Title = $"Page {level}";
        Heading.Text = Title;
        PopButton.IsVisible = level > 1;
        PopToRootButton.IsVisible = level > 2;

        // The back button on the next page shows this title on iOS and Mac.
        NavigationPage.SetBackButtonTitle(this, options.BackButtonTitle);

        if (level > 1)
        {
            NavigationPage.SetHasNavigationBar(this, options.HasNavigationBar);
            NavigationPage.SetHasBackButton(this, options.HasBackButton);
        }

        if (options.CustomTitleView && level == 1)
        {
            NavigationPage.SetTitleView(this, CreateTitleView());
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        DepthLabel.Text = $"NavigationStack.Count = {Navigation.NavigationStack.Count}";
    }

    private HorizontalStackLayout CreateTitleView()
    {
        var icon = new Image
        {
            Source = new FontImageSource
            {
                FontFamily = FluentIconsRegular.FontFamily,
                Glyph = FluentIconsRegular.MailInbox24,
                Color = Colors.Gray,
                Size = 20,
            },
            VerticalOptions = LayoutOptions.Center,
        };
        var title = new Label { Text = Title, FontAttributes = FontAttributes.Bold, VerticalOptions = LayoutOptions.Center };
        var subtitle = new Label { Text = "3 unread", FontSize = 12, Opacity = 0.7, VerticalOptions = LayoutOptions.Center };
        return new HorizontalStackLayout { Spacing = 8, HorizontalOptions = LayoutOptions.Center, Children = { icon, title, subtitle } };
    }

    private async void OnPushClicked(object? sender, EventArgs e) =>
        await Navigation.PushAsync(new NavigationDemoPage(_level + 1, _options));

    private async void OnPopClicked(object? sender, EventArgs e) => await Navigation.PopAsync();

    private async void OnPopToRootClicked(object? sender, EventArgs e) => await Navigation.PopToRootAsync();

    private async void OnCloseClicked(object? sender, EventArgs e) => await Navigation.PopModalAsync();
}
