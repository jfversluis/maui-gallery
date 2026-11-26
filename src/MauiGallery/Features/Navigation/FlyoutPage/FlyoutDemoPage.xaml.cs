namespace MauiGallery.Features.Navigation;

/// <summary>A mail-style flyout page that the flyout page sample presents modally.</summary>
public sealed partial class FlyoutDemoPage : FlyoutPage
{
    public FlyoutDemoPage()
    {
        InitializeComponent();
        Detail = CreateDetail("Inbox");
        MenuList.SelectedItem = "Inbox";
    }

    /// <summary>Raised when a menu item is selected, with its name.</summary>
    public event EventHandler<string>? ItemSelected;

    private void OnMenuSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.Count == 0 || e.CurrentSelection[0] is not string item || (Detail as NavigationPage)?.RootPage.Title == item)
        {
            return;
        }

        Detail = CreateDetail(item);
        ItemSelected?.Invoke(this, item);

        // A split layout keeps the flyout next to the detail, and IsPresented can't be set to false there.
        if (!((IFlyoutPageController)this).ShouldShowSplitMode)
        {
            IsPresented = false;
        }
    }

    private NavigationPage CreateDetail(string title)
    {
        var showMenu = new Button { Text = "Show the menu", AutomationId = "flyout-show-menu" };
        showMenu.Clicked += (_, _) => IsPresented = true;
        var close = new Button { Text = "Close", AutomationId = "flyout-close" };
        close.Clicked += OnCloseClicked;
        var heading = new Label { Text = title, FontSize = 28, FontAttributes = FontAttributes.Bold };
        SemanticProperties.SetHeadingLevel(heading, SemanticHeadingLevel.Level1);

        var page = new ContentPage
        {
            Title = title,
            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = 24,
                    Spacing = 16,
                    MaximumWidthRequest = 520,
                    Children =
                    {
                        heading,
                        new Label { Text = "This is the detail page. Open the menu with the button, the menu button in the navigation bar or, where enabled, a swipe from the edge." },
                        showMenu,
                        close,
                    },
                },
            },
        };
        return new NavigationPage(page);
    }

    private async void OnCloseClicked(object? sender, EventArgs e) => await Navigation.PopModalAsync();
}
