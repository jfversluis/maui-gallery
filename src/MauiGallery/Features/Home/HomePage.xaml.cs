namespace MauiGallery.Features.Home;

public sealed partial class HomePage : ContentPage
{
    /// <summary>Hero width above which the native showcase sits beside the text instead of below it.</summary>
    private const double SideBySideWidth = 820;

    public const string HeroCopy =
        "Explore every control, API and pattern in .NET MAUI. Each sample renders with the platform's own controls: change its options, read the code, and copy it into your app.";

    // Shorter on narrow screens, so the next section peeks above the phone's tab bar.
    private const string CompactHeroCopy = "Every control and API, rendered with the platform's own controls. Try the options, then copy the code.";

    private bool? _sideBySide;

    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        Hero.SizeChanged += (_, _) => UpdateHeroLayout();
    }

    private void UpdateHeroLayout()
    {
        var sideBySide = Hero.Width >= SideBySideWidth;
        if (_sideBySide == sideBySide)
        {
            return;
        }

        _sideBySide = sideBySide;
        HeroGrid.ColumnDefinitions.Clear();
        HeroGrid.RowDefinitions.Clear();
        if (sideBySide)
        {
            HeroGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(3, GridUnitType.Star)));
            HeroGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(2, GridUnitType.Star)));
            Grid.SetColumn(HeroShowcase, 1);
            Grid.SetRow(HeroShowcase, 0);
            Hero.Padding = new Thickness(48, 44);
            HeroTitle.FontSize = 40;
            HeroBody.Text = HeroCopy;
            HeroGrid.RowSpacing = 28;
            HeroText.Spacing = 16;
            ShowcaseItems.Spacing = 18;
            Sections.Spacing = 40;
        }
        else
        {
            HeroGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            HeroGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Grid.SetColumn(HeroShowcase, 0);
            Grid.SetRow(HeroShowcase, 1);
            Hero.Padding = new Thickness(24, 24);
            HeroTitle.FontSize = 32;
            HeroBody.Text = CompactHeroCopy;
            HeroGrid.RowSpacing = 20;
            HeroText.Spacing = 12;
            ShowcaseItems.Spacing = 12;
            Sections.Spacing = 32;
        }
    }
}
