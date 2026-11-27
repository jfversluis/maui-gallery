using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using MauiGallery.DesignSystem.Styles;
using MauiGallery.Services;
using Microsoft.Maui.Layouts;

namespace MauiGallery.DesignSystem.Components;

/// <summary>
/// The card that hosts one live sample: a neutral stage that renders the control at platform defaults, an options
/// pane to change it, an event log, and its XAML and C# source with live <c>$(Placeholder)</c> substitution.
/// </summary>
[ContentProperty(nameof(Example))]
public partial class ControlExample : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(ControlExample), string.Empty);

    public static readonly BindableProperty DescriptionProperty =
        BindableProperty.Create(nameof(Description), typeof(string), typeof(ControlExample));

    public static readonly BindableProperty ExampleProperty =
        BindableProperty.Create(nameof(Example), typeof(View), typeof(ControlExample), propertyChanged: OnExampleChanged);

    public static readonly BindableProperty OptionsProperty =
        BindableProperty.Create(nameof(Options), typeof(View), typeof(ControlExample), propertyChanged: OnOptionsChanged);

    public static readonly BindableProperty NativeTargetProperty =
        BindableProperty.Create(nameof(NativeTarget), typeof(View), typeof(ControlExample), propertyChanged: (b, _, _) => ((ControlExample)b).UpdateBadgeTarget());

    public static readonly BindableProperty ShowStyleToggleProperty =
        BindableProperty.Create(nameof(ShowStyleToggle), typeof(bool), typeof(ControlExample), true,
            propertyChanged: (b, _, _) => ((ControlExample)b).UpdateToolbar());

    public static readonly BindableProperty ShowRtlToggleProperty =
        BindableProperty.Create(nameof(ShowRtlToggle), typeof(bool), typeof(ControlExample), true,
            propertyChanged: (b, _, _) => ((ControlExample)b).UpdateToolbar());

    public static readonly BindableProperty ShowNativeBadgeProperty =
        BindableProperty.Create(nameof(ShowNativeBadge), typeof(bool), typeof(ControlExample), true,
            propertyChanged: (b, _, _) => ((ControlExample)b).UpdateToolbar());

    public static readonly BindableProperty NoticeProperty =
        BindableProperty.Create(nameof(Notice), typeof(string), typeof(ControlExample));

    public static readonly BindableProperty OutputProperty =
        BindableProperty.Create(nameof(Output), typeof(string), typeof(ControlExample));

    public static readonly BindableProperty XamlSourceProperty =
        BindableProperty.Create(nameof(XamlSource), typeof(string), typeof(ControlExample), propertyChanged: OnSourceChanged);

    public static readonly BindableProperty CSharpSourceProperty =
        BindableProperty.Create(nameof(CSharpSource), typeof(string), typeof(ControlExample), propertyChanged: OnSourceChanged);

    /// <summary>Width at which the options pane moves from below the stage to its right.</summary>
    private const double SideBySideWidth = 760;

    private const double StageMinimumHeight = 140;
    private const double CompactStageMinimumHeight = 120;

    private static readonly Lazy<SampleStyles> StyledLook = new(() => new SampleStyles());

    // An empty implicit style per styled control type. Swapping it in unapplies the styled setters; simply removing
    // the styled dictionary would leave its values on controls that were already styled.
    private static readonly Lazy<ResourceDictionary> NativeLook = new(() =>
    {
        var dictionary = new ResourceDictionary();
        foreach (var style in StyledLook.Value.Values.OfType<Style>())
        {
            dictionary.Add(new Style(style.TargetType));
        }

        return dictionary;
    });

    private bool? _sideBySide;
    private bool _isStyled;
    private bool _isQueued;
    private bool _isRealized;

    public ControlExample()
    {
        using var trace = PageLoadTrace.Measure("example");
        InitializeComponent();

        // The body (stage, options, output and code) holds most of the card's native views. It joins the page later,
        // in Realize, so the page can appear right away with a placeholder in its place.
        Substitutions.CollectionChanged += OnSubstitutionsChanged;
        if (Environment.GetEnvironmentVariable("MG_NO_DEFER_REALIZE") == "1")
        {
            _isRealized = true;
            CardLayout.Remove(Placeholder);
            Loaded += (_, _) =>
            {
                LastRealized = Title;
                RenderCode();
                NameOptionControls();
            };
        }
        else
        {
            CardLayout.Remove(Body);
            Loaded += (_, _) => QueueRealize();
        }
        UpdateStageMode();
        UpdateLayout();
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string? Description
    {
        get => (string?)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    /// <summary>The live sample. Placed on the stage exactly as written.</summary>
    public View? Example
    {
        get => (View?)GetValue(ExampleProperty);
        set => SetValue(ExampleProperty, value);
    }

    /// <summary>Controls that change the sample's properties.</summary>
    public View? Options
    {
        get => (View?)GetValue(OptionsProperty);
        set => SetValue(OptionsProperty, value);
    }

    /// <summary>The control whose native type is shown in the badge. Defaults to <see cref="Example"/>.</summary>
    public View? NativeTarget
    {
        get => (View?)GetValue(NativeTargetProperty);
        set => SetValue(NativeTargetProperty, value);
    }

    /// <summary>
    /// Whether the Default/Styled toggle is shown. Turn it off for examples that set appearance properties
    /// explicitly, where an app-wide style would not change anything.
    /// </summary>
    public bool ShowStyleToggle
    {
        get => (bool)GetValue(ShowStyleToggleProperty);
        set => SetValue(ShowStyleToggleProperty, value);
    }

    /// <summary>
    /// Whether the LTR/RTL toggle is shown. Turn it off when mirroring means nothing, for example when the stage
    /// only holds a button that calls an API.
    /// </summary>
    public bool ShowRtlToggle
    {
        get => (bool)GetValue(ShowRtlToggleProperty);
        set => SetValue(ShowRtlToggleProperty, value);
    }

    /// <summary>
    /// Whether the badge with the sample's native type is shown. Turn it off when the stage only triggers an API and
    /// the native control isn't the point.
    /// </summary>
    public bool ShowNativeBadge
    {
        get => (bool)GetValue(ShowNativeBadgeProperty);
        set => SetValue(ShowNativeBadgeProperty, value);
    }

    /// <summary>
    /// A short callout above the stage for prerequisites or unsupported states, such as "iOS only". Keep longer
    /// explanations in the description.
    /// </summary>
    public string? Notice
    {
        get => (string?)GetValue(NoticeProperty);
        set => SetValue(NoticeProperty, value);
    }

    /// <summary>A short log line describing the last event the sample raised.</summary>
    public string? Output
    {
        get => (string?)GetValue(OutputProperty);
        set => SetValue(OutputProperty, value);
    }

    /// <summary>Name of the embedded XAML snippet, for example <c>ButtonBasic.xaml.txt</c>.</summary>
    public string? XamlSource
    {
        get => (string?)GetValue(XamlSourceProperty);
        set => SetValue(XamlSourceProperty, value);
    }

    /// <summary>Name of the embedded C# snippet, for example <c>ButtonBasic.cs.txt</c>.</summary>
    public string? CSharpSource
    {
        get => (string?)GetValue(CSharpSourceProperty);
        set => SetValue(CSharpSourceProperty, value);
    }

    public ObservableCollection<Substitution> Substitutions { get; } = [];

    public static string? LastRealized { get; private set; }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateLayout();
    }

    /// <summary>
    /// Queues the example to be realized. Examples realize one at a time, a frame apart and in page order, so the page
    /// appears before all of its examples are built and stays responsive while they fill in.
    /// </summary>
    private void QueueRealize()
    {
        if (_isRealized || _isQueued)
        {
            return;
        }

        _isQueued = true;
        DeferredWork.Enqueue(Dispatcher, () =>
        {
            _isQueued = false;

            // An example on a page that was closed in the meantime waits until it is shown again.
            if (!IsLoaded || _isRealized)
            {
                return false;
            }

            Realize();
            return true;
        });
    }

    private void Realize()
    {
        using var trace = PageLoadTrace.Measure("realize");
        _isRealized = true;
        LastRealized = Title;
        if (Environment.GetEnvironmentVariable("MG_OLD_REALIZE") != "1" && Handler?.MauiContext is { } context)
        {
            Microsoft.Maui.Platform.ElementExtensions.ToPlatform(Body, context);
        }

        CardLayout.Remove(Placeholder);
        CardLayout.Add(Body);
        RenderCode();
        NameOptionControls();
    }

    private static void OnExampleChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        var example = (ControlExample)bindable;
        example.StageHost.Content = newValue as View;
        example.UpdateBadgeTarget();
    }

    private static void OnOptionsChanged(BindableObject bindable, object? oldValue, object? newValue)
    {
        var example = (ControlExample)bindable;
        example.OptionsHost.Content = newValue as View;
        example._sideBySide = null;
        example.UpdateLayout();
    }

    /// <summary>
    /// Gives each options control without an accessible name the text of the label shown before it, so screen readers
    /// announce "CornerRadius, 8" instead of an unnamed slider. A trailing ": value" in the label is left out because
    /// the control announces its own value.
    /// </summary>
    private void NameOptionControls()
    {
        string? label = null;
        Visit(OptionsHost);

        void Visit(IVisualTreeElement element)
        {
            foreach (var child in element.GetVisualChildren())
            {
                switch (child)
                {
                    case Label { Text: { Length: > 0 } text }:
                        label = text.Split(':')[0].Trim();
                        break;
                    case View view and (InputView or Slider or Switch or CheckBox or Stepper or Picker or DatePicker or TimePicker or SwatchPicker)
                        when label is not null && string.IsNullOrEmpty(SemanticProperties.GetDescription(view)):
                        SemanticProperties.SetDescription(view, label);
                        break;
                    default:
                        Visit(child);
                        break;
                }
            }
        }
    }

    private static void OnSourceChanged(BindableObject bindable, object? oldValue, object? newValue) =>
        ((ControlExample)bindable).RenderCode();

    private void OnSubstitutionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var item in e.OldItems?.OfType<Substitution>() ?? [])
        {
            item.PropertyChanged -= OnSubstitutionPropertyChanged;
        }

        foreach (var item in e.NewItems?.OfType<Substitution>() ?? [])
        {
            item.PropertyChanged += OnSubstitutionPropertyChanged;
        }

        RenderCode();
    }

    private void OnSubstitutionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Substitution.Value))
        {
            RenderCode();
        }
    }

    private void RenderCode()
    {
        // Properties and substitutions arrive in XAML order; render once everything is in place.
        if (!_isRealized)
        {
            return;
        }

        Code.SetCode(Render(XamlSource, s => s.XamlText), Render(CSharpSource, s => s.CSharpText));
        var hasCode = Code.Xaml is not null || Code.CSharp is not null;
        if (CodeArea.IsVisible != hasCode)
        {
            CodeArea.IsVisible = hasCode;
            _sideBySide = null;
            UpdateLayout();
        }
    }

    private string? Render(string? source, Func<Substitution, string?> format)
    {
        if (string.IsNullOrEmpty(source))
        {
            return null;
        }

        var values = new Dictionary<string, string?>(StringComparer.Ordinal);
        foreach (var substitution in Substitutions)
        {
            values[substitution.Key] = format(substitution);
        }

        return EmbeddedSnippets.Get(source).Render(values);
    }

    private void UpdateBadgeTarget() => Badge.Target = NativeTarget ?? Example;

    private void UpdateToolbar()
    {
        Badge.IsVisible = ShowNativeBadge;
        StyleToggle.IsVisible = ShowStyleToggle;
        RtlToggle.IsVisible = ShowRtlToggle;
        ToolbarToggles.IsVisible = ShowStyleToggle || ShowRtlToggle;
        Toolbar.IsVisible = ShowNativeBadge || ToolbarToggles.IsVisible;
        Toolbar.JustifyContent = ShowNativeBadge ? FlexJustify.SpaceBetween : FlexJustify.End;
        StageHost.Padding = Toolbar.IsVisible ? new Thickness(24, 16, 24, 32) : new Thickness(24, 28);
    }

    private void UpdateLayout()
    {
        var hasOptions = Options is not null;
        var wide = Width >= SideBySideWidth;
        var sideBySide = hasOptions && wide;
        Stage.MinimumHeightRequest = wide ? StageMinimumHeight : CompactStageMinimumHeight;
        Placeholder.HeightRequest = Stage.MinimumHeightRequest;
        if (_sideBySide == sideBySide && Body.ColumnDefinitions.Count > 0)
        {
            return;
        }

        _sideBySide = sideBySide;
        OptionsPanel.IsVisible = hasOptions;
        OptionsDivider.IsVisible = hasOptions;
        Body.ColumnDefinitions.Clear();
        Body.RowDefinitions.Clear();

        if (sideBySide)
        {
            // The options rail runs beside the stage, output and code. The code row takes up any extra height, so
            // tall options never stretch the stage itself. Without code, the stage takes it instead.
            var stretchStage = !CodeArea.IsVisible;
            Body.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            Body.ColumnDefinitions.Add(new ColumnDefinition((double)Application.Current!.Resources["OptionsPaneWidth"]));
            Body.RowDefinitions.Add(new RowDefinition(stretchStage ? GridLength.Star : GridLength.Auto));
            Body.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Body.RowDefinitions.Add(new RowDefinition(stretchStage ? GridLength.Auto : GridLength.Star));
            Place(Stage, row: 0, column: 0);
            Place(OutputRow, row: 1, column: 0);
            Place(CodeArea, row: 2, column: 0);
            Place(OptionsPanel, row: 0, column: 1, rowSpan: 3);
            Place(OptionsDivider, row: 0, column: 1, rowSpan: 3);
            OptionsDivider.WidthRequest = 1;
            OptionsDivider.HeightRequest = -1;
            OptionsDivider.HorizontalOptions = LayoutOptions.Start;
            OptionsDivider.VerticalOptions = LayoutOptions.Fill;
        }
        else
        {
            Body.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            for (var i = 0; i < 4; i++)
            {
                Body.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }

            Place(Stage, row: 0, column: 0);
            Place(OptionsPanel, row: 1, column: 0);
            Place(OptionsDivider, row: 1, column: 0);
            Place(OutputRow, row: 2, column: 0);
            Place(CodeArea, row: 3, column: 0);
            OptionsDivider.WidthRequest = -1;
            OptionsDivider.HeightRequest = 1;
            OptionsDivider.HorizontalOptions = LayoutOptions.Fill;
            OptionsDivider.VerticalOptions = LayoutOptions.Start;
        }

        void Place(View view, int row, int column, int rowSpan = 1)
        {
            Body.SetRow(view, row);
            Body.SetColumn(view, column);
            Body.SetRowSpan(view, rowSpan);
        }
    }

    private void UpdateStageMode()
    {
        StageHost.Resources = _isStyled ? StyledLook.Value : NativeLook.Value;
        NativeModeButton.Style = (Style)Resources[_isStyled ? "SegmentButton" : "SegmentButtonSelected"];
        StyledModeButton.Style = (Style)Resources[_isStyled ? "SegmentButtonSelected" : "SegmentButton"];
        SemanticProperties.SetDescription(NativeModeButton, _isStyled ? "Default look" : "Default look, selected");
        SemanticProperties.SetDescription(StyledModeButton, _isStyled ? "Styled look, selected" : "Styled look");
    }

    private void OnNativeModeClicked(object? sender, EventArgs e)
    {
        _isStyled = false;
        UpdateStageMode();
    }

    private void OnStyledModeClicked(object? sender, EventArgs e)
    {
        _isStyled = true;
        UpdateStageMode();
    }

    private void OnRtlClicked(object? sender, EventArgs e)
    {
        var rtl = StageHost.FlowDirection != FlowDirection.RightToLeft;

        // Explicitly LeftToRight, not back to MatchParent: on iOS and Mac Catalyst, .NET MAUI only passes an explicit
        // direction on to child views, so after MatchParent the sample would keep its inherited right-to-left layout.
        StageHost.FlowDirection = rtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        RtlButton.Text = rtl ? "RTL" : "LTR";
        RtlButton.Style = (Style)Resources[rtl ? "SegmentButtonSelected" : "SegmentButton"];
        SemanticProperties.SetDescription(RtlButton, rtl ? "Layout direction: right to left" : "Layout direction: left to right");
    }
}
