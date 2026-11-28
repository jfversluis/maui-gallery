using MauiGallery.Core.Layout;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Navigation;

public sealed partial class ShellSamplePage : SamplePage, IQueryAttributable
{
    private static readonly Dictionary<string, string> RouteHints = new(StringComparer.Ordinal)
    {
        ["button"] = "Relative: pushes the Button page onto this stack. Back returns here.",
        ["//main/controls/button"] = "Absolute: replaces the stack with Controls > Button.",
        ["//main/settings"] = "Absolute: switches to the Settings section.",
        ["//main/home"] = "Absolute: switches to the Home section.",
    };

    private Shell? _shell;

    public ShellSamplePage(SamplePageServices services)
        : base(services)
    {
        // Registering the same route and page type again is allowed, so this is safe for every new instance.
        Routing.RegisterRoute(ShellSwatchPage.Route, typeof(ShellSwatchPage));

        InitializeComponent();

        RoutesPicker.SelectedIndex = 0;
        ParametersColor.SelectedIndex = 0;
        ChromeFlyout.SelectedIndex = 0;
        RoutesExample.NativeTarget = RoutesButton;
        ParametersExample.NativeTarget = ParametersButton;
        Loaded += (_, _) => Attach();
    }

    /// <summary>Receives the reply that <see cref="ShellSwatchPage"/> sends back when it navigates to "..".</summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("reply", out var reply))
        {
            ParametersReply.Text = $"Reply: {reply}";
            ParametersExample.Output = $"ApplyQueryAttributes: reply = \"{reply}\"";
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // The app shell sets every open page's tab bar when the window crosses a breakpoint, also while this page is
        // covered, so apply this page's own choice again.
        ApplyTabBarChoice();
        Attach();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (_shell is not null)
        {
            _shell.Navigated -= OnShellNavigated;
            _shell = null;
        }

        if (Window is { } window)
        {
            window.SizeChanged -= OnWindowSizeChanged;
        }
    }

    /// <summary>
    /// Starts following the Shell and the window. A page pushed onto a Shell stack can appear before it's attached to
    /// the window, so this runs again once the page has loaded.
    /// </summary>
    private void Attach()
    {
        if (_shell is null && Window?.Page is Shell shell)
        {
            _shell = shell;
            _shell.Navigated += OnShellNavigated;
            Window.SizeChanged += OnWindowSizeChanged;
        }

        UpdateLiveValues();
        UpdateChromeStatus();
    }

    private void OnShellNavigated(object? sender, ShellNavigatedEventArgs e) => UpdateLiveValues();

    private void OnWindowSizeChanged(object? sender, EventArgs e)
    {
        // The app shell handles the resize first and sets this page's tab bar to the window's default.
        ApplyTabBarChoice();
        UpdateLiveValues();
        UpdateChromeStatus();
    }

    private void ApplyTabBarChoice()
    {
        if (!ChromeTabBar.IsToggled)
        {
            Shell.SetTabBarIsVisible(this, false);
        }
    }

    private void UpdateLiveValues()
    {
        if (_shell is null || Window is not { } window)
        {
            return;
        }

        RoutesLocation.Text = _shell.CurrentState.Location.OriginalString;

        var tab = _shell.CurrentItem?.CurrentItem;
        AdaptiveWidth.Text = $"{window.Width:0} ({Breakpoints.Classify(window.Width)})";
        AdaptiveFlyout.Text = _shell.FlyoutBehavior.ToString();
        AdaptiveTabs.Text = tab is null ? "-" : Shell.GetTabBarIsVisible(tab).ToString();
        AdaptiveDepth.Text = $"{Navigation.NavigationStack.Count} pages";
        AdaptiveHint.Text = AppShell.IsDesktop
            ? $"Resize the window: the sidebar becomes a flyout below {Breakpoints.Expanded:0} wide."
            : $"Bottom tabs below {Breakpoints.Medium:0} wide or {Breakpoints.CompactHeight:0} high, a sidebar above that: rotate the device, open a foldable or use split view.";
    }

    private void OnRoutesPickerChanged(object? sender, EventArgs e) =>
        RoutesHint.Text = RoutesPicker.SelectedItem is string route ? RouteHints[route] : null;

    private async void OnRoutesClicked(object? sender, EventArgs e)
    {
        if (_shell is not null && RoutesPicker.SelectedItem is string route)
        {
            await _shell.GoToAsync(route);
        }
    }

    private async void OnParametersClicked(object? sender, EventArgs e)
    {
        if (_shell is null || ParametersColor.SelectedItem is not string color)
        {
            return;
        }

        // .NET 11 resolves route templates in absolute URIs only, so build one from the current location.
        var note = ParametersNote.Text ?? string.Empty;
        var here = _shell.CurrentState.Location.OriginalString;
        var route = $"{here}/swatch/{color}?note={Uri.EscapeDataString(note)}";
        ParametersExample.Output = $"GoToAsync(\"{route}\")";
        await _shell.GoToAsync(route);
    }

    private void OnChromeChanged(object? sender, EventArgs e)
    {
        Shell.SetNavBarIsVisible(this, ChromeNavBar.IsToggled);

        // Clear rather than set True: a value on the page would override the app hiding the tabs on wide windows.
        if (ChromeTabBar.IsToggled)
        {
            ClearValue(Shell.TabBarIsVisibleProperty);
        }
        else
        {
            Shell.SetTabBarIsVisible(this, false);
        }

        if (ChromeFlyout.SelectedItem is string choice && Enum.TryParse<FlyoutBehavior>(choice, out var behavior))
        {
            Shell.SetFlyoutBehavior(this, behavior);
            ChromeFlyoutSubstitution.Value = behavior.ToString();
            ChromeFlyoutCodeSubstitution.Value = $"FlyoutBehavior.{behavior}";
        }
        else
        {
            ClearValue(Shell.FlyoutBehaviorProperty);
            ChromeFlyoutSubstitution.Value = null;
            ChromeFlyoutCodeSubstitution.Value = null;
        }

        UpdateChromeStatus();
    }

    private void UpdateChromeStatus()
    {
        var flyout = IsSet(Shell.FlyoutBehaviorProperty)
            ? Shell.GetFlyoutBehavior(this).ToString()
            : $"not set, so the app's {_shell?.FlyoutBehavior.ToString() ?? "value"} applies";
        ChromeStatus.Text = $"Navigation bar: {(ChromeNavBar.IsToggled ? "visible" : "hidden")}\n" +
                            $"Tab bar: {(ChromeTabBar.IsToggled ? "visible" : "hidden")}\n" +
                            $"Flyout: {flyout}";
        ChromeOpenFlyout.IsVisible = IsSet(Shell.FlyoutBehaviorProperty) && Shell.GetFlyoutBehavior(this) == FlyoutBehavior.Flyout;
        var windowShowsTabs = _shell?.CurrentItem?.CurrentItem is { } tab && Shell.GetTabBarIsVisible(tab);
        ChromeHint.Text = windowShowsTabs
            ? "With the navigation bar hidden, use the back gesture to leave. Flyout turns the app's sidebar into a flyout on this page."
            : "This window shows the sidebar instead of tabs, so TabBarIsVisible only matters in a compact window. Disabled hides the sidebar; Locked keeps it open.";
    }

    // The back button takes the place of the flyout button on a pushed page, so open the flyout from code.
    private void OnOpenFlyoutClicked(object? sender, EventArgs e)
    {
        if (_shell is not null)
        {
            _shell.FlyoutIsPresented = true;
            ChromeExample.Output = "FlyoutIsPresented = true";
        }
    }
}
