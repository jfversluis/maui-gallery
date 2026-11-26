using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Layouts;

public sealed partial class ScrollViewPage : SamplePage
{
    public ScrollViewPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        BindableLayout.SetItemsSource(VerticalItems, Enumerable.Range(1, 20).ToArray());
        BindableLayout.SetItemsSource(OrientationCells, CreateCells());
        BindableLayout.SetItemsSource(ScrollToItems, Enumerable.Range(1, 30).ToArray());
        VerticalExample.NativeTarget = VerticalSample;
        OrientationExample.NativeTarget = OrientationSample;
        ScrollToExample.NativeTarget = ScrollToScroller;
        OnScrollToItemChanged(this, EventArgs.Empty);
    }

    // A 10 × 10 grid of spreadsheet-style cells: A1 to J10.
    private static string[] CreateCells() =>
        [.. from row in Enumerable.Range(1, 10) from column in "ABCDEFGHIJ" select $"{column}{row}"];

    private void OnVerticalScrolled(object? sender, ScrolledEventArgs e) =>
        VerticalExample.Output = $"Scrolled: ScrollY = {e.ScrollY:0}";

    private void OnOrientationScrolled(object? sender, ScrolledEventArgs e) =>
        OrientationExample.Output = $"Scrolled: ScrollX = {e.ScrollX:0}, ScrollY = {e.ScrollY:0}";

    private void OnScrollToItemChanged(object? sender, EventArgs e) =>
        ScrollToIndex.Value = (int)ScrollToItem.Value - 1;

    private async void OnScrollToItemClicked(object? sender, EventArgs e)
    {
        var item = (Element)ScrollToItems.Children[(int)ScrollToItem.Value - 1];
        var position = (ScrollToPosition)ScrollToPositionPicker.SelectedItem;
        await ScrollToScroller.ScrollToAsync(item, position, ScrollToAnimated.IsToggled);
        ScrollToExample.Output = $"ScrollToAsync(item {ScrollToItem.Value:0}, {position}) finished at ScrollY = {ScrollToScroller.ScrollY:0}";
    }

    private async void OnBackToTopClicked(object? sender, EventArgs e)
    {
        await ScrollToScroller.ScrollToAsync(0, 0, ScrollToAnimated.IsToggled);
        ScrollToExample.Output = $"ScrollToAsync(0, 0) finished at ScrollY = {ScrollToScroller.ScrollY:0}";
    }
}
