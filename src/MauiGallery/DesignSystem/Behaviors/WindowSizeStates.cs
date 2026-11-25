using MauiGallery.Core.Layout;

namespace MauiGallery.DesignSystem.Behaviors;

/// <summary>
/// Moves an element between the <c>Compact</c>, <c>Medium</c> and <c>Expanded</c> visual states as its window is resized,
/// using the <see cref="Breakpoints"/> the rest of the app uses.
/// </summary>
/// <remarks>
/// This replaces <see cref="AdaptiveTrigger"/> in shared styles. MAUI gives every element that uses a style the same
/// trigger instances, so only the most recently styled element follows the window (dotnet/maui#14259). Here each
/// element subscribes to its own window.
/// </remarks>
public static class WindowSizeStates
{
    public static readonly BindableProperty IsEnabledProperty =
        BindableProperty.CreateAttached("IsEnabled", typeof(bool), typeof(WindowSizeStates), false, propertyChanged: OnIsEnabledChanged);

    private static readonly BindableProperty SubscriptionProperty =
        BindableProperty.CreateAttached("Subscription", typeof(Subscription), typeof(WindowSizeStates), null);

    public static bool GetIsEnabled(BindableObject element) => (bool)element.GetValue(IsEnabledProperty);

    public static void SetIsEnabled(BindableObject element, bool value) => element.SetValue(IsEnabledProperty, value);

    private static void OnIsEnabledChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is not VisualElement element)
        {
            return;
        }

        element.PropertyChanged -= OnElementPropertyChanged;
        Unsubscribe(element);

        if ((bool)newValue)
        {
            element.PropertyChanged += OnElementPropertyChanged;
            Subscribe(element);
        }
    }

    private static void OnElementPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VisualElement.Window) && sender is VisualElement element)
        {
            Unsubscribe(element);
            Subscribe(element);
        }
    }

    private static void Subscribe(VisualElement element)
    {
        if (element.Window is not { } window)
        {
            return;
        }

        // The handler only holds the element weakly, so a window outliving a page does not keep the page alive.
        var target = new WeakReference<VisualElement>(element);
        void OnSizeChanged(object? sender, EventArgs e)
        {
            if (target.TryGetTarget(out var current))
            {
                Update(current, window);
            }
        }

        window.SizeChanged += OnSizeChanged;
        element.SetValue(SubscriptionProperty, new Subscription(window, OnSizeChanged));
        Update(element, window);
    }

    private static void Unsubscribe(VisualElement element)
    {
        if (element.GetValue(SubscriptionProperty) is Subscription subscription)
        {
            subscription.Window.SizeChanged -= subscription.Handler;
            element.ClearValue(SubscriptionProperty);
        }
    }

    private static void Update(VisualElement element, Window window)
    {
        if (window.Width > 0)
        {
            VisualStateManager.GoToState(element, Breakpoints.Classify(window.Width).ToString());
        }
    }

    private sealed record Subscription(Window Window, EventHandler Handler);
}
