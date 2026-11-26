using System.Windows.Input;

namespace MauiGallery.Features.Fundamentals;

/// <summary>Runs a command when the element loses focus: an event-to-command behavior for one event.</summary>
public sealed class UnfocusedCommandBehavior : Behavior<VisualElement>
{
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(UnfocusedCommandBehavior));

    public static readonly BindableProperty CommandParameterProperty =
        BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(UnfocusedCommandBehavior));

    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    protected override void OnAttachedTo(VisualElement bindable)
    {
        base.OnAttachedTo(bindable);

        // Behaviors aren't part of the visual tree, so they don't inherit a BindingContext. Borrow the element's.
        BindingContext = bindable.BindingContext;
        bindable.BindingContextChanged += OnBindingContextChanged;
        bindable.Unfocused += OnUnfocused;
    }

    protected override void OnDetachingFrom(VisualElement bindable)
    {
        bindable.BindingContextChanged -= OnBindingContextChanged;
        bindable.Unfocused -= OnUnfocused;
        BindingContext = null;
        base.OnDetachingFrom(bindable);
    }

    private void OnBindingContextChanged(object? sender, EventArgs e) => BindingContext = ((BindableObject)sender!).BindingContext;

    private void OnUnfocused(object? sender, FocusEventArgs e)
    {
        if (Command?.CanExecute(CommandParameter) == true)
        {
            Command.Execute(CommandParameter);
        }
    }
}
