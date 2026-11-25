using MauiGallery.Core.Layout;
using MauiGallery.Features.Chrome;

namespace MauiGallery;

public partial class AppShell : Shell
{
    private readonly SidebarView _sidebar;
    private readonly SidebarViewModel _sidebarViewModel;

    public AppShell(SidebarView sidebar, AppShellViewModel viewModel)
    {
        InitializeComponent();
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
        }

        _sidebar.FocusSearch();
    }

    private void OnSearchClicked(object? sender, EventArgs e)
    {
        if (FlyoutBehavior == FlyoutBehavior.Disabled)
        {
            _ = GoToAsync(Features.Catalog.Routes.Search);
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

        // With the sidebar, root pages need neither tabs nor a title bar; pushed pages opt back in for Back.
        foreach (var tab in Items.SelectMany(item => item.Items))
        {
            SetTabBarIsVisible(tab, useTabs);
            SetNavBarIsVisible(tab, useTabs);
        }
    }

    protected override void OnNavigated(ShellNavigatedEventArgs args)
    {
        base.OnNavigated(args);
        _sidebarViewModel.UpdateSelection(CurrentState.Location.OriginalString);
        if (FlyoutBehavior == FlyoutBehavior.Flyout)
        {
            FlyoutIsPresented = false;
        }
    }
}
