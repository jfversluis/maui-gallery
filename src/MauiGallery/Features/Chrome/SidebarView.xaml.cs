namespace MauiGallery.Features.Chrome;

/// <summary>The desktop and tablet navigation pane: app identity, search and the catalog, grouped by category.</summary>
public partial class SidebarView : ContentView
{
    public SidebarView(SidebarViewModel viewModel)
    {
        InitializeComponent();
        ViewModel = viewModel;
        BindingContext = viewModel;
        viewModel.SelectionChanged += OnSelectionChanged;
    }

    public SidebarViewModel ViewModel { get; }

    /// <summary>Moves keyboard focus to the search box (Ctrl+K / Cmd+K).</summary>
    public void FocusSearch() => SearchEntry.Focus();

    // Runs after the selected entry's group has expanded and laid out, so the row has its final position.
    private void OnSelectionChanged(object? sender, SidebarEntry entry) =>
        Dispatcher.DispatchDelayed(TimeSpan.FromMilliseconds(50), () => _ = ScrollIntoViewAsync(entry));

    private async Task ScrollIntoViewAsync(SidebarEntry entry)
    {
        var row = NavItems.GetVisualTreeDescendants()
            .OfType<Border>()
            .FirstOrDefault(b => ReferenceEquals(b.BindingContext, entry) && b.Height > 0);
        if (row is null || NavScroll.Height <= 0)
        {
            return;
        }

        var visible = NavScroll.GetScrollPositionForElement(row, ScrollToPosition.MakeVisible);
        if (visible.Y.Equals(NavScroll.ScrollY))
        {
            return;
        }

        var centered = NavScroll.GetScrollPositionForElement(row, ScrollToPosition.Center);
        var maxY = Math.Max(0, NavScroll.ContentSize.Height - NavScroll.Height);
        await NavScroll.ScrollToAsync(0, Math.Clamp(centered.Y, 0, maxY), animated: true);
    }
}
