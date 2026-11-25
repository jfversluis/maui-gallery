namespace MauiGallery.Features.Chrome;

/// <summary>The desktop and tablet navigation pane: app identity, search and the catalog, grouped by category.</summary>
public partial class SidebarView : ContentView
{
    public SidebarView(SidebarViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        BindingContext = viewModel;
    }

    public SidebarViewModel ViewModel { get; }

    /// <summary>Moves keyboard focus to the search box (Ctrl+K / Cmd+K).</summary>
    public void FocusSearch() => SearchEntry.Focus();
}
