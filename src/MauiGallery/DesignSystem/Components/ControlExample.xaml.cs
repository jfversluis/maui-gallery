using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using MauiGallery.DesignSystem.Styles;

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
            propertyChanged: (b, _, value) => ((ControlExample)b).StyleToggle.IsVisible = (bool)value);

    public static readonly BindableProperty OutputProperty =
        BindableProperty.Create(nameof(Output), typeof(string), typeof(ControlExample));

    public static readonly BindableProperty XamlSourceProperty =
        BindableProperty.Create(nameof(XamlSource), typeof(string), typeof(ControlExample), propertyChanged: OnSourceChanged);

    public static readonly BindableProperty CSharpSourceProperty =
        BindableProperty.Create(nameof(CSharpSource), typeof(string), typeof(ControlExample), propertyChanged: OnSourceChanged);

    /// <summary>Width at which the options pane moves from below the stage to its right.</summary>
    private const double SideBySideWidth = 760;

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

    public ControlExample()
    {
        InitializeComponent();
        Substitutions.CollectionChanged += OnSubstitutionsChanged;
        Loaded += (_, _) => RenderCode();
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

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateLayout();
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
        if (!IsLoaded)
        {
            return;
        }

        Code.Xaml = Render(XamlSource, s => s.XamlText);
        Code.CSharp = Render(CSharpSource, s => s.CSharpText);
        Code.IsVisible = Code.Xaml is not null || Code.CSharp is not null;
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

    private void UpdateLayout()
    {
        var hasOptions = Options is not null;
        var sideBySide = hasOptions && Width >= SideBySideWidth;
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
            Body.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            Body.ColumnDefinitions.Add(new ColumnDefinition((double)Application.Current!.Resources["OptionsPaneWidth"]));
            Body.SetRow(OptionsPanel, 0);
            Body.SetColumn(OptionsPanel, 1);
            Body.SetRow(OptionsDivider, 0);
            Body.SetColumn(OptionsDivider, 1);
            OptionsDivider.WidthRequest = 1;
            OptionsDivider.HeightRequest = -1;
            OptionsDivider.HorizontalOptions = LayoutOptions.Start;
            OptionsDivider.VerticalOptions = LayoutOptions.Fill;
        }
        else
        {
            Body.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            Body.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Body.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            Body.SetRow(OptionsPanel, 1);
            Body.SetColumn(OptionsPanel, 0);
            Body.SetRow(OptionsDivider, 1);
            Body.SetColumn(OptionsDivider, 0);
            OptionsDivider.WidthRequest = -1;
            OptionsDivider.HeightRequest = 1;
            OptionsDivider.HorizontalOptions = LayoutOptions.Fill;
            OptionsDivider.VerticalOptions = LayoutOptions.Start;
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
        StageHost.FlowDirection = rtl ? FlowDirection.RightToLeft : FlowDirection.MatchParent;
        RtlButton.Text = rtl ? "RTL" : "LTR";
        RtlButton.Style = (Style)Resources[rtl ? "SegmentButtonSelected" : "SegmentButton"];
        SemanticProperties.SetDescription(RtlButton, rtl ? "Layout direction: right to left" : "Layout direction: left to right");
    }
}
