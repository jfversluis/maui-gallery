using IconFont.Maui.FluentIcons;
using MauiGallery.DesignSystem.Components;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Navigation;

public sealed partial class TitleBarPage : SamplePage
{
    private Window? _window;
    private TitleBar? _titleBar;
    private TitleBarWindowPage? _windowPage;
    private ImageButton? _backButton;
    private SearchBar? _searchBar;
    private ImageButton? _accountButton;

    public TitleBarPage(SamplePageServices services)
        : base(services)
    {
        ArgumentNullException.ThrowIfNull(services);
        InitializeComponent();

        BasicForeground.SelectedIndex = 0;
        BasicBackground.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SwatchPicker.SelectedColor))
            {
                Apply();
            }
        };

        var supported = DeviceInfo.Platform == DevicePlatform.MacCatalyst || DeviceInfo.Platform == DevicePlatform.WinUI;
        BasicButton.IsEnabled = supported && services.Windows.CanOpenWindows;
        ContentButton.IsEnabled = BasicButton.IsEnabled;
        Apply();
    }

    private Color? SelectedForeground => (BasicForeground.SelectedItem as string) switch
    {
        "White" => Colors.White,
        "Black" => Colors.Black,
        _ => null,
    };

    private void OnOpenClicked(object? sender, EventArgs e)
    {
        var example = sender == ContentButton ? ContentExample : BasicExample;
        if (_window is not null)
        {
            Application.Current?.ActivateWindow(_window);
            example.Output = "ActivateWindow: the title bar window is already open";
            return;
        }

        _titleBar = new TitleBar();
        _windowPage = new TitleBarWindowPage();
        _backButton = CreateBackButton();
        _searchBar = CreateSearchBar();
        _accountButton = CreateAccountButton();
        _window = new Window(_windowPage)
        {
            TitleBar = _titleBar,
        };
        _window.Destroying += OnWindowDestroying;
        Apply();

        Application.Current?.OpenWindow(_window);
        example.Output = "OpenWindow: a new window with Window.TitleBar set";
    }

    private void OnWindowDestroying(object? sender, EventArgs e)
    {
        if (_window is not null)
        {
            _window.Destroying -= OnWindowDestroying;
        }

        _window = null;
        _titleBar = null;
        _windowPage = null;
        _backButton = null;
        _searchBar = null;
        _accountButton = null;
        BasicExample.Output = "The title bar window closed";
    }

    private void OnOptionChanged(object? sender, EventArgs e) => Apply();

    /// <summary>Updates the code and, when its window is open, the live title bar.</summary>
    private void Apply()
    {
        var foreground = SelectedForeground;
        BasicIconSubstitution.Value = BasicIcon.IsToggled ? "mail.png" : null;
        BasicIconCodeSubstitution.Value = BasicIcon.IsToggled ? "\"mail.png\"" : null;
        BasicForegroundSubstitution.Value = foreground is null ? null : BasicForeground.SelectedItem;
        BasicForegroundCodeSubstitution.Value = foreground is null ? null : $"Colors.{BasicForeground.SelectedItem}";
        ContentLeadingSubstitution.Value = ContentLeading.IsToggled ? "backButton" : null;
        ContentSearchSubstitution.Value = ContentSearch.IsToggled ? "searchBar" : null;
        ContentTrailingSubstitution.Value = ContentTrailing.IsToggled ? "accountButton" : null;

        if (_titleBar is not { } titleBar || _window is not { } window || _backButton is null || _accountButton is null)
        {
            return;
        }

        window.Title = BasicTitle.Text;
        titleBar.Title = BasicTitle.Text;
        titleBar.Subtitle = BasicSubtitle.Text;
        titleBar.BackgroundColor = BasicBackground.SelectedColor;
        SetOrClear(titleBar, TitleBar.IconProperty, BasicIcon.IsToggled ? Glyph(FluentIconsRegular.MailInbox24, foreground) : null);
        SetOrClear(titleBar, TitleBar.ForegroundColorProperty, foreground);
        _backButton.Source = Glyph(FluentIconsRegular.ChevronLeft24, foreground);
        _accountButton.Source = Glyph(FluentIconsRegular.PersonCircle24, foreground);
        titleBar.LeadingContent = ContentLeading.IsToggled ? _backButton : null;
        titleBar.Content = ContentSearch.IsToggled ? _searchBar : null;
        titleBar.TrailingContent = ContentTrailing.IsToggled ? _accountButton : null;

        // A search box needs more room than the standard 32-point title bar.
        titleBar.HeightRequest = ContentSearch.IsToggled ? 48 : -1;
    }

    private ImageButton CreateBackButton()
    {
        var button = new ImageButton
        {
            WidthRequest = 32,
            HeightRequest = 32,
            Padding = 6,
            AutomationId = "titlebar-back",
        };
        SemanticProperties.SetDescription(button, "Back");
        button.Clicked += (_, _) => Report("Back clicked");
        return button;
    }

    private SearchBar CreateSearchBar()
    {
        var search = new SearchBar
        {
            Placeholder = "Search mail",
            BackgroundColor = Colors.Transparent,
            MaximumWidthRequest = 320,
            VerticalOptions = LayoutOptions.Center,
            AutomationId = "titlebar-search",
        };
        search.SearchButtonPressed += (_, _) => Report($"Searched for \"{search.Text}\"");
        return search;
    }

    private ImageButton CreateAccountButton()
    {
        var button = new ImageButton
        {
            WidthRequest = 32,
            HeightRequest = 32,
            Padding = 4,
            AutomationId = "titlebar-account",
        };
        SemanticProperties.SetDescription(button, "Account");
        button.Clicked += (_, _) => Report("Account clicked");
        return button;
    }

    private void Report(string text)
    {
        _windowPage?.ShowStatus(text);
        ContentExample.Output = text;
    }

    private static void SetOrClear(BindableObject target, BindableProperty property, object? value)
    {
        if (value is null)
        {
            target.ClearValue(property);
        }
        else
        {
            target.SetValue(property, value);
        }
    }

    private static FontImageSource Glyph(string glyph, Color? color) => new()
    {
        FontFamily = FluentIconsRegular.FontFamily,
        Glyph = glyph,
        Color = color ?? Colors.Gray,
        Size = 18,
    };
}
