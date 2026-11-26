using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Collections;

public sealed partial class SwipeViewPage : SamplePage
{
    public SwipeViewPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        var inbox = new InboxViewModel();
        inbox.ActionInvoked += (_, message) => InboxExample.Output = message;
        InboxSample.BindingContext = inbox;
        InboxRestore.Command = inbox.RestoreCommand;
        InboxExample.NativeTarget = InboxLayout.Children.OfType<SwipeView>().FirstOrDefault();

        var cart = new CartLineViewModel();
        cart.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(CartLineViewModel.Quantity))
            {
                CartExample.Output = $"Quantity: {cart.Quantity}";
            }
        };
        CartSample.BindingContext = cart;
        CartExample.NativeTarget = CartSwipe;
    }

    // The options apply to every row, including rows the BindableLayout creates later.
    private void OnInboxOptionChanged(object? sender, EventArgs e)
    {
        foreach (var row in InboxLayout.Children.OfType<SwipeView>())
        {
            ApplyInboxOptions(row);
        }
    }

    private void OnInboxRowAdded(object? sender, ElementEventArgs e)
    {
        if (e.Element is SwipeView row)
        {
            ApplyInboxOptions(row);
        }
    }

    private void ApplyInboxOptions(SwipeView row)
    {
        if (InboxMode is null || InboxBehavior is null)
        {
            return;
        }

        var behavior = (SwipeBehaviorOnInvoked)InboxBehavior.SelectedItem;
        row.LeftItems.Mode = (SwipeMode)InboxMode.SelectedItem;
        row.LeftItems.SwipeBehaviorOnInvoked = behavior;
        row.RightItems.SwipeBehaviorOnInvoked = behavior;
    }

    private void OnCodeSwipeStarted(object? sender, SwipeStartedEventArgs e) =>
        CodeExample.Output = $"SwipeStarted: {e.SwipeDirection}";

    private void OnCodeSwipeEnded(object? sender, SwipeEndedEventArgs e) =>
        CodeExample.Output = $"SwipeEnded: {e.SwipeDirection}, IsOpen = {e.IsOpen}";

    private void OnCodeOpenRequested(object? sender, OpenRequestedEventArgs e) =>
        CodeExample.Output = $"OpenRequested: {e.OpenSwipeItem}";

    private void OnCodeCloseRequested(object? sender, CloseRequestedEventArgs e) =>
        CodeExample.Output = "CloseRequested";

    private void OnCodeItemInvoked(object? sender, EventArgs e)
    {
        if (sender is SwipeItem item)
        {
            CodeExample.Output = $"Invoked: {item.Text}";
        }
    }

    private void OnOpenLeftClicked(object? sender, EventArgs e) => CodeSwipe.Open(OpenSwipeItem.LeftItems);

    private void OnOpenRightClicked(object? sender, EventArgs e) => CodeSwipe.Open(OpenSwipeItem.RightItems);

    private void OnCloseClicked(object? sender, EventArgs e) => CodeSwipe.Close();
}
