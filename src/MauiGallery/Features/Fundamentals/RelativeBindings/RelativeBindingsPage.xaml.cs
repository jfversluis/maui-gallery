using System.Collections.Specialized;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class RelativeBindingsPage : SamplePage
{
    public RelativeBindingsPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        SelfBox.SetBinding(BoxView.HeightRequestProperty, static (BoxView box) => box.WidthRequest, source: RelativeBindingSource.Self);

        var shoppingList = new ShoppingListViewModel();
        shoppingList.Items.CollectionChanged += OnShoppingListChanged;
        AncestorSample.BindingContext = shoppingList;
    }

    private void OnShoppingListChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
        AncestorExample.Output = e.Action switch
        {
            NotifyCollectionChangedAction.Remove => $"RemoveCommand ran on the page's view model with \"{e.OldItems?[0]}\"",
            NotifyCollectionChangedAction.Reset => "ResetCommand ran",
            _ => AncestorExample.Output,
        };
}
