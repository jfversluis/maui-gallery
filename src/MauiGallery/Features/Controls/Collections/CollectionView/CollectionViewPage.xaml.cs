using System.ComponentModel;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Collections;

public sealed partial class CollectionViewPage : SamplePage
{
    private readonly OrderFeedViewModel _orders = new();

    public CollectionViewPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        EmptySample.BindingContext = new PlaceSearchViewModel();
        EmptyExample.NativeTarget = EmptyCollection;
        LoadMoreCollection.BindingContext = _orders;
        LoadMoreReset.Command = _orders.ResetCommand;
        _orders.PropertyChanged += OnOrdersPropertyChanged;
    }

    private void OnLayoutChanged(object? sender, EventArgs e)
    {
        var span = (int)Math.Round(LayoutSpan.Value);
        var (layout, xaml, code, height) = LayoutPicker.SelectedIndex switch
        {
            1 => ((IItemsLayout)new GridItemsLayout(span, ItemsLayoutOrientation.Vertical), $"VerticalGrid, {span}", $"new GridItemsLayout({span}, ItemsLayoutOrientation.Vertical)", 320),
            2 => (LinearItemsLayout.Horizontal, "HorizontalList", "LinearItemsLayout.Horizontal", 64),
            3 => (new GridItemsLayout(span, ItemsLayoutOrientation.Horizontal), $"HorizontalGrid, {span}", $"new GridItemsLayout({span}, ItemsLayoutOrientation.Horizontal)", 64 * span),
            _ => (LinearItemsLayout.Vertical, "VerticalList", "LinearItemsLayout.Vertical", 320),
        };

        LayoutCollection.ItemsLayout = layout;
        LayoutCollection.HeightRequest = height;
        LayoutSpan.IsEnabled = layout is GridItemsLayout;
        LayoutXamlSubstitution.Value = xaml;
        LayoutCodeSubstitution.Value = code;
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var names = e.CurrentSelection.OfType<Place>().Select(p => p.Name).ToList();
        SelectionExample.Output = SelectionCollection.SelectionMode == SelectionMode.Multiple
            ? $"SelectedItems ({names.Count}): {(names.Count == 0 ? "none" : string.Join(", ", names))}"
            : $"SelectedItem: {(names.Count == 0 ? "none" : names[0])}";
    }

    private void OnClearSelectionClicked(object? sender, EventArgs e)
    {
        SelectionCollection.SelectedItem = null;
        SelectionCollection.SelectedItems?.Clear();
    }

    private void OnOrdersPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(OrderFeedViewModel.IsLoading))
        {
            LoadMoreExample.Output = _orders.IsLoading
                ? "RemainingItemsThresholdReached: loading the next page"
                : $"Loaded {OrderFeedViewModel.PageSize} more orders ({_orders.Orders.Count} of {OrderFeedViewModel.TotalOrders})";
        }
    }
}
