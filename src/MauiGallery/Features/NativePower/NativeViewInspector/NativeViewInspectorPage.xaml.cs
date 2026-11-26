using MauiGallery.DesignSystem.Components;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.NativePower;

public sealed partial class NativeViewInspectorPage : SamplePage
{
    // The controls to inspect: picker name, the C# shown in the snippet, and a factory for the live view.
    private static readonly (string Name, string Code, Func<View> Create)[] Inspectables =
    [
        ("Button", "Button { Text = \"Inspect me\" }", () => new Button { Text = "Inspect me" }),
        ("Label", "Label { Text = \"Inspect me\" }", () => new Label { Text = "Inspect me" }),
        ("Entry", "Entry { Placeholder = \"Inspect me\" }", () => new Entry { Placeholder = "Inspect me", WidthRequest = 200 }),
        ("Switch", "Switch { IsToggled = true }", () => new Switch { IsToggled = true }),
        ("Slider", "Slider { Value = 0.4 }", () => new Slider { Value = 0.4, WidthRequest = 200 }),
        ("CheckBox", "CheckBox { IsChecked = true }", () => new CheckBox { IsChecked = true }),
        ("DatePicker", "DatePicker()", () => new DatePicker()),
        ("ProgressBar", "ProgressBar { Progress = 0.6 }", () => new ProgressBar { Progress = 0.6, WidthRequest = 200 }),
    ];

    private const string ShadowCode = "new Shadow { Radius = 12, Opacity = 0.4f, Offset = new Point(0, 4) }";

    // The base classes every native view shares; the chain stops after these.
    private static readonly HashSet<string> RootTypes = ["UIKit.UIView", "Android.Views.View", "Microsoft.UI.Xaml.UIElement"];

    private View? _inspected;

    public NativeViewInspectorPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        InspectorControl.ItemsSource = Inspectables.Select(i => i.Name).ToArray();
        InspectorControl.SelectedIndex = 0;
    }

    private void OnInspectorChanged(object? sender, EventArgs e)
    {
        if (InspectorControl.SelectedIndex < 0)
        {
            return;
        }

        if (_inspected is not null)
        {
            _inspected.HandlerChanged -= OnInspectedHandlerChanged;
        }

        var (_, code, create) = Inspectables[InspectorControl.SelectedIndex];
        var view = create();
        if (InspectorShadow.IsToggled)
        {
            view.Shadow = new Shadow { Radius = 12, Opacity = 0.4f, Offset = new Point(0, 4) };
        }

        view.AutomationId = "sample-inspector-view";
        view.HandlerChanged += OnInspectedHandlerChanged;
        _inspected = view;
        InspectorHost.Content = view;
        InspectorExample.NativeTarget = view;
        InspectorElement.Value = code;
        InspectorShadowSubstitution.Value = InspectorShadow.IsToggled ? ShadowCode : null;
    }

    private void OnInspectedHandlerChanged(object? sender, EventArgs e)
    {
        if (sender is not View { Handler: IViewHandler { PlatformView: { } platformView } handler })
        {
            return;
        }

        HandlerName.Text = handler.GetType().Name;
        PlatformChain.Text = string.Join(Environment.NewLine, TypeChain(platformView.GetType()));
        ContainerName.Text = handler.ContainerView?.GetType().Name ?? "None";
        InspectorExample.Output = $"Added to the native layout: {(handler.ContainerView ?? platformView).GetType().Name}";
    }

    /// <summary>The type and its base types, up to the platform's root view type.</summary>
    private static IEnumerable<string> TypeChain(Type type)
    {
        var prefix = string.Empty;
        for (Type? current = type; current is not null; current = current.BaseType)
        {
            var owner = current.Namespace?.StartsWith("Microsoft.Maui", StringComparison.Ordinal) == true
                ? ".NET MAUI"
                : NativeViewBadge.FrameworkName(current.Namespace);
            yield return $"{prefix}{current.Name} ({owner})";

            if (RootTypes.Contains(current.FullName ?? string.Empty))
            {
                yield break;
            }

            prefix = "↳ ";
        }
    }
}
