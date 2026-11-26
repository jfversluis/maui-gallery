using MauiGallery.Core.Layout;
using MauiGallery.Features.Chrome;
using MauiGallery.Services;

namespace MauiGallery;

public partial class AppShell : Shell
{
    private readonly SidebarView _sidebar;
    private readonly SidebarViewModel _sidebarViewModel;
    private readonly AppShellViewModel _viewModel;

    public AppShell(SidebarView sidebar, AppShellViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        _sidebar = sidebar;
        _sidebarViewModel = sidebar.ViewModel;
        FlyoutContent = sidebar;
    }

    /// <summary>Moves focus to the sidebar search box, if the sidebar is showing.</summary>
    public void FocusSearch()
    {
        if (FlyoutBehavior == FlyoutBehavior.Flyout)
        {
            FlyoutIsPresented = true;

            // Focus once the flyout has been presented; focusing a hidden field is ignored.
            Dispatcher.Dispatch(_sidebar.FocusSearch);
            return;
        }

        _sidebar.FocusSearch();
    }

    /// <summary>
    /// With the sidebar, root pages need no title bar: the sidebar shows where you are. Pushed pages keep it for Back.
    /// Unlike TabBarIsVisible, NavBarIsVisible is not inherited from the Tab, so it is set on the current page.
    /// </summary>
    private void UpdateNavBar()
    {
        if (CurrentPage is { } page && page.Navigation.NavigationStack.Count <= 1)
        {
            SetNavBarIsVisible(page, FlyoutBehavior != FlyoutBehavior.Locked);
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
    }

    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);
        PageLoadTrace.Begin(args.Target.Location.OriginalString);
    }

    protected override void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);
        PageLoadTrace.Mark("navigated");
        TraceFirstLayout(CurrentPage);
        UpdateNavBar();
        _sidebarViewModel.UpdateSelection(CurrentState.Location.OriginalString);
        if (FlyoutBehavior == FlyoutBehavior.Flyout)
        {
            FlyoutIsPresented = false;
        }
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
