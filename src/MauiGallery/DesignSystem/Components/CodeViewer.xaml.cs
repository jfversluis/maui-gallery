using MauiGallery.Core.Syntax;

namespace MauiGallery.DesignSystem.Components;

/// <summary>Shows syntax-highlighted XAML and C# with tabs and a copy button. Colors follow the active theme.</summary>
public partial class CodeViewer : ContentView
{
    public static readonly BindableProperty XamlProperty =
        BindableProperty.Create(nameof(Xaml), typeof(string), typeof(CodeViewer), propertyChanged: OnCodeChanged);

    public static readonly BindableProperty CSharpProperty =
        BindableProperty.Create(nameof(CSharp), typeof(string), typeof(CodeViewer), propertyChanged: OnCodeChanged);

    private static readonly TimeSpan CopiedFeedbackDuration = TimeSpan.FromSeconds(1.5);

    private CodeLanguage _language = CodeLanguage.Xaml;

    public CodeViewer()
    {
        InitializeComponent();
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

    private static void OnCodeChanged(BindableObject bindable, object oldValue, object newValue) => ((CodeViewer)bindable).Render();

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

    private void Render()
    {
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

        var code = CurrentCode ?? string.Empty;
        var text = new FormattedString();
        foreach (var token in SyntaxHighlighter.Tokenize(code, _language))
        {
            var span = new Span { Text = token.Text };
            span.SetDynamicResource(Span.TextColorProperty, ResourceKeyFor(token.Kind));
            text.Spans.Add(span);
        }

        CodeLabel.FormattedText = text;
        CodeLabel.SetValue(SemanticProperties.DescriptionProperty, code);
        LineNumbers.Text = string.Join('\n', Enumerable.Range(1, code.Count(c => c == '\n') + 1));
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

    private async void OnCopyClicked(object? sender, EventArgs e)
    {
        await Clipboard.Default.SetTextAsync(CurrentCode);
        CopyGlyph.Glyph = IconFont.Maui.FluentIcons.FluentIconsRegular.Checkmark24;
        SemanticScreenReader.Announce("Code copied");
        await Task.Delay(CopiedFeedbackDuration);
        CopyGlyph.Glyph = IconFont.Maui.FluentIcons.FluentIconsRegular.Copy24;
    }
}
