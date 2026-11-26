using MauiGallery.Core.Layout;
using MauiGallery.Features.Chrome;
using MauiGallery.Services;

namespace MauiGallery;

public partial class AppShell : Shell
{
    private readonly Func<SidebarView> _createSidebar;
    private readonly AppShellViewModel _viewModel;
    private SidebarView? _sidebar;
    private Page? _leavingPage;

    // On Windows the menu bar (Shell.MenuBarItems) is part of the navigation bar, so that bar stays visible.
    private static readonly bool MenuBarInNavBar = OperatingSystem.IsWindows();

    /// <param name="createSidebar">
    /// Creates the sidebar. Phones navigate with tabs and never show it, so it is only created once a window needs it.
    /// </param>
    /// <param name="viewModel">The menu bar's commands.</param>
    public AppShell(Func<SidebarView> createSidebar, AppShellViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        _createSidebar = createSidebar;
        InitializePlatformChrome();
    }

    /// <summary>Platform-specific window chrome, set up once (see Platforms/Windows/AppShell.Windows.cs).</summary>
    partial void InitializePlatformChrome();

    /// <summary>Updates the platform-specific chrome for the current <see cref="Shell.FlyoutBehavior"/>.</summary>
    partial void ApplyPlatformLayout();

    /// <summary>Moves focus to the sidebar search box, if the sidebar is showing.</summary>
    public void FocusSearch()
    {
        if (FlyoutBehavior == FlyoutBehavior.Flyout)
        {
            FlyoutIsPresented = true;

            // Focus once the flyout has been presented; focusing a hidden field is ignored.
            Dispatcher.Dispatch(() => _sidebar?.FocusSearch());
            return;
        }

        _sidebar?.FocusSearch();
    }

    /// <summary>
    /// With the sidebar, root pages need no title bar: the sidebar shows where you are. Pushed pages keep it for Back.
    /// Unlike TabBarIsVisible, NavBarIsVisible is not inherited from the Tab, so it is set on the current page.
    /// </summary>
    private void UpdateNavBar()
    {
        if (CurrentPage is { } page && page.Navigation.NavigationStack.Count <= 1)
        {
            SetNavBarIsVisible(page, FlyoutBehavior != FlyoutBehavior.Locked || MenuBarInNavBar);
        }
    }

    private void OnSearchClicked(object? sender, EventArgs e)
    {
        if (FlyoutBehavior == FlyoutBehavior.Disabled)
        {
            _viewModel.GoToCommand.Execute(Features.Catalog.Routes.Search);
            return;
        }

        FocusSearch();
    }

    /// <summary>
    /// Chooses the navigation chrome for the window width. Called by the window on every resize, because Shell
    /// itself is never measured and so never raises <c>SizeChanged</c>.
    /// </summary>
    public void ApplyLayout(double windowWidth)
    {
        var useTabs = DeviceInfo.Idiom == DeviceIdiom.Phone;
        var behavior = useTabs ? FlyoutBehavior.Disabled
            : Breakpoints.Classify(windowWidth) == SizeClass.Expanded ? FlyoutBehavior.Locked
            : FlyoutBehavior.Flyout;

        if (!useTabs && _sidebar is null)
        {
            _sidebar = _createSidebar();
            FlyoutContent = _sidebar;
            if (CurrentState is { } state)
            {
                _sidebar.ViewModel.UpdateSelection(state.Location.OriginalString);
            }
        }

        if (FlyoutBehavior != behavior)
        {
            FlyoutBehavior = behavior;
            FlyoutIsPresented = false;
        }

        foreach (var tab in Items.SelectMany(item => item.Items))
        {
            SetTabBarIsVisible(tab, useTabs);
        }

        UpdateNavBar();
        ApplyPlatformLayout();
    }

    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);
        PageLoadTrace.Begin(args.Target.Location.OriginalString);
        DeferDisconnect(CurrentPage);
    }

    protected override void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);
        PageLoadTrace.Mark("navigated");
        DisconnectLeavingPage();
        TraceFirstLayout(CurrentPage);
        UpdateNavBar();
        _sidebar?.ViewModel.UpdateSelection(CurrentState.Location.OriginalString);
        if (FlyoutBehavior == FlyoutBehavior.Flyout)
        {
            FlyoutIsPresented = false;
        }
    }

    /// <summary>
    /// Shell disconnects the handlers of a page it removes right away, tearing down all of its native views before the
    /// next page can appear. The page being left keeps its handlers until the new page is on screen instead.
    /// </summary>
    private void DeferDisconnect(Page? page)
    {
        if (_leavingPage is not null)
        {
            HandlerProperties.SetDisconnectPolicy(_leavingPage, HandlerDisconnectPolicy.Automatic);
        }

        _leavingPage = page;
        if (page is not null)
        {
            HandlerProperties.SetDisconnectPolicy(page, HandlerDisconnectPolicy.Manual);
        }
    }

    private void DisconnectLeavingPage()
    {
        if (_leavingPage is not { } page)
        {
            return;
        }

        _leavingPage = null;
        HandlerProperties.SetDisconnectPolicy(page, HandlerDisconnectPolicy.Automatic);

        // Still in a navigation stack (a page was pushed on top of it, or another tab was selected): nothing to do.
        if (page.Parent is not null)
        {
            return;
        }

        DeferredWork.Enqueue(Dispatcher, () =>
        {
            if (page.Parent is not null || page.Handler is null)
            {
                return false;
            }

            page.DisconnectHandlers();
            return true;
        });
    }

    /// <summary>Ends the page load measurement once the page has been laid out and the UI thread is free again.</summary>
    private void TraceFirstLayout(Page? page)
    {
        if (page is null || page.Width > 0)
        {
            Dispatcher.Dispatch(() => PageLoadTrace.End("ready"));
            return;
        }

        page.SizeChanged += OnFirstLayout;

        void OnFirstLayout(object? sender, EventArgs e)
        {
            page.SizeChanged -= OnFirstLayout;
            PageLoadTrace.Mark("layout");
            Dispatcher.Dispatch(() => PageLoadTrace.End("ready"));
        }
    }
}
