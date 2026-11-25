namespace MauiGallery.DesignSystem.Components;

/// <summary>
/// Shows the platform control that .NET MAUI created for <see cref="Target"/>, read at runtime from
/// <c>Handler.PlatformView</c>. MAUI's own thin wrappers (for example <c>MauiTextField</c>) are skipped so the
/// badge names the real platform type (<c>UITextField</c>).
/// </summary>
public partial class NativeViewBadge : ContentView
{
    public static readonly BindableProperty TargetProperty =
        BindableProperty.Create(nameof(Target), typeof(View), typeof(NativeViewBadge), propertyChanged: OnTargetChanged);

    public NativeViewBadge() => InitializeComponent();

    public View? Target
    {
        get => (View?)GetValue(TargetProperty);
        set => SetValue(TargetProperty, value);
    }

    /// <summary>Returns the first type in the inheritance chain that is not part of .NET MAUI itself.</summary>
    internal static Type PlatformTypeOf(Type type)
    {
        var current = type;
        while (current.BaseType is { } baseType && current.Namespace?.StartsWith("Microsoft.Maui", StringComparison.Ordinal) == true)
        {
            current = baseType;
        }

        return current;
    }

    private static void OnTargetChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        var badge = (NativeViewBadge)bindable;
        if (oldValue is View oldView)
        {
            oldView.HandlerChanged -= badge.OnTargetHandlerChanged;
        }

        if (newValue is View newView)
        {
            newView.HandlerChanged += badge.OnTargetHandlerChanged;
        }

        badge.Update();
    }

    private void OnTargetHandlerChanged(object? sender, EventArgs e) => Update();

    private void Update()
    {
        if (Target?.Handler?.PlatformView is not { } platformView)
        {
            Pill.IsVisible = false;
            return;
        }

        var actualType = platformView.GetType();
        var platformType = PlatformTypeOf(actualType);
        TypeNameLabel.Text = platformType.Name;
        NamespaceLabel.Text = platformType.Namespace;
        Pill.IsVisible = true;

        var description = actualType == platformType
            ? $"Rendered natively as {platformType.FullName}"
            : $"Rendered natively as {platformType.FullName} (through {actualType.Name})";
        ToolTipProperties.SetText(Pill, description);
        SemanticProperties.SetDescription(Pill, description);
    }
}
