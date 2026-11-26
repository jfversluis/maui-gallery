using IconFont.Maui.FluentIcons;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.DesignGuidance;

public sealed partial class IconographyPage : SamplePage
{
    // Star16, Star20, Star24 and Star32: each size has its own drawing.
    private static readonly Dictionary<int, (string Regular, string Filled)> Stars = new()
    {
        [16] = (FluentIconsRegular.Star16, FluentIconsFilled.Star16),
        [20] = (FluentIconsRegular.Star20, FluentIconsFilled.Star20),
        [24] = (FluentIconsRegular.Star24, FluentIconsFilled.Star24),
        [32] = (FluentIconsRegular.Star32, FluentIconsFilled.Star32),
    };

    public IconographyPage(SamplePageServices services, IClipboard clipboard)
        : base(services)
    {
        InitializeComponent();
        Browser.BindingContext = new IconBrowserViewModel(clipboard);
        UpdateFavorite();
    }

    private void OnFavoriteClicked(object? sender, EventArgs e)
    {
        FavoriteSwitch.IsToggled = !FavoriteSwitch.IsToggled;
        FavoriteExample.Output = $"Clicked: favorite is {(FavoriteSwitch.IsToggled ? "on" : "off")}";
    }

    private void OnFavoriteOptionChanged(object? sender, EventArgs e) => UpdateFavorite();

    private void UpdateFavorite()
    {
        // Options raise their events while the page is still loading.
        if (FavoriteButton is null || FavoriteSwitch is null || FavoriteSize?.SelectedItem is not int size)
        {
            return;
        }

        var isFavorite = FavoriteSwitch.IsToggled;
        var (regular, filled) = Stars[size];
        var source = new FontImageSource
        {
            FontFamily = isFavorite ? FluentIconsFilled.FontFamily : FluentIconsRegular.FontFamily,
            Glyph = isFavorite ? filled : regular,
            Size = size,
        };
        source.SetAppThemeColor(
            FontImageSource.ColorProperty,
            Color.FromArgb(isFavorite ? "#512BD4" : "#6B677D"),
            Color.FromArgb(isFavorite ? "#B3A3FF" : "#B4AFC4"));

        FavoriteButton.Source = source;
        SemanticProperties.SetDescription(FavoriteButton, isFavorite ? "Favorite, on" : "Favorite");
    }
}
