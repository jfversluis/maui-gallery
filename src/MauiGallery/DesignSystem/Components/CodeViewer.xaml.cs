using MauiGallery.Core.Syntax;
using MauiGallery.Services;

namespace MauiGallery.DesignSystem.Components;

/// <summary>Shows syntax-highlighted XAML and C# with tabs and a copy button. Colors follow the active theme.</summary>
public partial class CodeViewer : ContentView
{
    public static readonly BindableProperty XamlProperty =
        BindableProperty.Create(nameof(Xaml), typeof(string), typeof(CodeViewer), propertyChanged: OnCodeChanged);

    public static readonly BindableProperty CSharpProperty =
        BindableProperty.Create(nameof(CSharp), typeof(string), typeof(CodeViewer), propertyChanged: OnCodeChanged);

    private static readonly TimeSpan CopiedFeedbackDuration = TimeSpan.FromSeconds(1.5);

    // One bindable property per syntax color, bound to the theme's resource once per viewer. A DynamicResource on
    // each of the hundreds of spans made resolving resources most of the cost of showing code.
    private static readonly Dictionary<string, BindableProperty> TokenColorProperties = Enum.GetValues<SyntaxTokenKind>()
        .Select(ResourceKeyFor)
        .Distinct()
        .ToDictionary(key => key, key => BindableProperty.Create(key, typeof(Color), typeof(CodeViewer), propertyChanged: OnTokenColorChanged));

    /// <summary>Long snippets start collapsed to this many lines.</summary>
    private const int CollapsedLines = 14;

    // Collapsing only pays off when it hides a good part of the snippet.
    private const int CollapseAbove = 18;

    private CodeLanguage _language = CodeLanguage.Xaml;
    private bool _isExpanded;
    private bool _isSettingCode;
    private bool _colorsChanged;
    private (string Code, CodeLanguage Language, bool Collapsed)? _rendered;

    public CodeViewer()
    {
        InitializeComponent();
        foreach (var (key, property) in TokenColorProperties)
        {
            SetDynamicResource(property, key);
        }

        Render();
    }

    public string? Xaml
    {
        get => (string?)GetValue(XamlProperty);
        set => SetValue(XamlProperty, value);
    }

    public string? CSharp
    {
        get => (string?)GetValue(CSharpProperty);
        set => SetValue(CSharpProperty, value);
    }

    private string? CurrentCode => _language == CodeLanguage.Xaml ? Xaml : CSharp;

    /// <summary>Sets both snippets and renders once.</summary>
    public void SetCode(string? xaml, string? csharp)
    {
        _isSettingCode = true;
        try
        {
            Xaml = xaml;
            CSharp = csharp;
        }
        finally
        {
            _isSettingCode = false;
        }

        Render();
    }

    private static void OnCodeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var viewer = (CodeViewer)bindable;
        if (!viewer._isSettingCode)
        {
            viewer.Render();
        }
    }

    // A theme switch changes every color at once: redraw the code once, after all of them have changed.
    private static void OnTokenColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var viewer = (CodeViewer)bindable;
        if (viewer._rendered is null || viewer._colorsChanged)
        {
            return;
        }

        viewer._colorsChanged = true;
        viewer.Dispatcher.Dispatch(() =>
        {
            viewer._colorsChanged = false;
            viewer._rendered = null;
            viewer.Render();
        });
    }

    private static string ResourceKeyFor(SyntaxTokenKind kind) => kind switch
    {
        SyntaxTokenKind.Punctuation => "ColorSyntaxPunctuation",
        SyntaxTokenKind.Comment or SyntaxTokenKind.Preprocessor => "ColorSyntaxComment",
        SyntaxTokenKind.Keyword => "ColorSyntaxKeyword",
        SyntaxTokenKind.StringLiteral or SyntaxTokenKind.AttributeValue => "ColorSyntaxString",
        SyntaxTokenKind.Number => "ColorSyntaxNumber",
        SyntaxTokenKind.TypeName => "ColorSyntaxType",
        SyntaxTokenKind.Method => "ColorSyntaxMethod",
        SyntaxTokenKind.ElementName => "ColorSyntaxElement",
        SyntaxTokenKind.AttributeName => "ColorSyntaxAttribute",
        SyntaxTokenKind.MarkupExtension => "ColorSyntaxMarkup",
        _ => "ColorSyntaxPlain",
    };

    private Color? ColorFor(SyntaxTokenKind kind) => (Color?)GetValue(TokenColorProperties[ResourceKeyFor(kind)]);

    private void Render()
    {
        using var trace = PageLoadTrace.Measure("code");
        var hasXaml = !string.IsNullOrEmpty(Xaml);
        var hasCSharp = !string.IsNullOrEmpty(CSharp);
        if (_language == CodeLanguage.Xaml && !hasXaml && hasCSharp)
        {
            _language = CodeLanguage.CSharp;
        }

        XamlTab.IsVisible = hasXaml;
        CSharpTab.IsVisible = hasCSharp;
        XamlTab.Style = (Style)Resources[_language == CodeLanguage.Xaml ? "SelectedTab" : "Tab"];
        CSharpTab.Style = (Style)Resources[_language == CodeLanguage.CSharp ? "SelectedTab" : "Tab"];
        SemanticProperties.SetDescription(XamlTab, _language == CodeLanguage.Xaml ? "XAML code, selected" : "XAML code");
        SemanticProperties.SetDescription(CSharpTab, _language == CodeLanguage.CSharp ? "C sharp code, selected" : "C sharp code");

        var code = CurrentCode ?? string.Empty;
        var lineCount = SyntaxHighlighter.CountLines(code);
        var collapsible = lineCount > CollapseAbove;
        var collapsed = collapsible && !_isExpanded;

        // Changing an option often changes only the snippet in the other tab; rebuilding the text is the costly part.
        if (_rendered != (code, _language, collapsed))
        {
            _rendered = (code, _language, collapsed);
            var tokens = SyntaxHighlighter.Tokenize(code, _language);
            if (collapsed)
            {
                tokens = SyntaxHighlighter.TakeLines(tokens, CollapsedLines);
            }

            var text = new FormattedString();
            foreach (var token in SyntaxHighlighter.FoldWhitespace(tokens))
            {
                text.Spans.Add(new Span { Text = token.Text, TextColor = ColorFor(token.Kind) });
            }

            CodeLabel.FormattedText = text;
            CodeLabel.SetValue(SemanticProperties.DescriptionProperty, code);
            LineNumbers.Text = string.Join('\n', Enumerable.Range(1, collapsed ? CollapsedLines : lineCount));
        }

        ExpandButton.IsVisible = collapsible;
        CollapsedFade.IsVisible = collapsed;
        ExpandButton.Text = collapsed ? $"Show full code ({lineCount} lines)" : "Show less";
        ExpandGlyph.Glyph = collapsed ? IconFont.Maui.FluentIcons.FluentIconsRegular.ChevronDown24 : IconFont.Maui.FluentIcons.FluentIconsRegular.ChevronUp24;
    }

    private void OnExpandClicked(object? sender, EventArgs e)
    {
        _isExpanded = !_isExpanded;
        Render();
    }

    private void OnXamlTabClicked(object? sender, EventArgs e)
    {
        _language = CodeLanguage.Xaml;
        Render();
    }

    private void OnCSharpTabClicked(object? sender, EventArgs e)
    {
        _language = CodeLanguage.CSharp;
        Render();
    }

    private void OnCodeLayoutChanged(object? sender, EventArgs e) =>
        OverflowFade.IsVisible = CodeContent.Width - CodeScroll.ScrollX - CodeScroll.Width > 1;

    private async void OnCopyClicked(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(CurrentCode))
        {
            return;
        }

        await Clipboard.Default.SetTextAsync(CurrentCode);
        CopyGlyph.Glyph = IconFont.Maui.FluentIcons.FluentIconsRegular.Checkmark24;
        SemanticScreenReader.Announce("Code copied");
        await Task.Delay(CopiedFeedbackDuration);
        CopyGlyph.Glyph = IconFont.Maui.FluentIcons.FluentIconsRegular.Copy24;
    }
}
