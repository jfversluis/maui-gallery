using MauiGallery.Core.Syntax;

namespace MauiGallery.Core.Tests;

public sealed class SyntaxHighlighterTests
{
    private static SyntaxTokenKind KindOf(IReadOnlyList<SyntaxToken> tokens, string text) =>
        tokens.Single(t => t.Text == text).Kind;

    [Theory]
    [InlineData(CodeLanguage.Xaml, """<Button Text="Hi" Clicked="{Binding Go}" /><!-- c --> text""")]
    [InlineData(CodeLanguage.Xaml, "<Grid\n    x:Name=\"Root\">\n</Grid")]
    [InlineData(CodeLanguage.CSharp, "var s = $\"{a}\" + @\"x\"\"y\"; // done\n#if DEBUG\nFoo(1.5f, 'c');")]
    [InlineData(CodeLanguage.CSharp, "/* unterminated")]
    [InlineData(CodeLanguage.CSharp, "\"unterminated")]
    public void TokensReproduceSourceExactly(CodeLanguage language, string source)
    {
        var tokens = SyntaxHighlighter.Tokenize(source, language);

        Assert.Equal(source, string.Concat(tokens.Select(t => t.Text)));
    }

    [Fact]
    public void XamlIsClassified()
    {
        var tokens = SyntaxHighlighter.Tokenize("""<Button Text="Hi" Command="{Binding Save}" />""", CodeLanguage.Xaml);

        Assert.Equal(SyntaxTokenKind.ElementName, KindOf(tokens, "Button"));
        Assert.Equal(SyntaxTokenKind.AttributeName, KindOf(tokens, "Text"));
        Assert.Equal(SyntaxTokenKind.AttributeValue, KindOf(tokens, "\"Hi\""));
        Assert.Equal(SyntaxTokenKind.MarkupExtension, KindOf(tokens, "\"{Binding Save}\""));
        Assert.Equal(SyntaxTokenKind.Punctuation, KindOf(tokens, "/>"));
    }

    [Fact]
    public void XamlCommentsAreClassified()
    {
        var tokens = SyntaxHighlighter.Tokenize("<!-- note --><Label />", CodeLanguage.Xaml);

        Assert.Equal(SyntaxTokenKind.Comment, tokens[0].Kind);
    }

    [Fact]
    public void CSharpIsClassified()
    {
        var tokens = SyntaxHighlighter.Tokenize("var button = new Button(); button.Focus(); // hi", CodeLanguage.CSharp);

        Assert.Equal(SyntaxTokenKind.Keyword, KindOf(tokens, "var"));
        Assert.Equal(SyntaxTokenKind.Plain, tokens.First(t => t.Text.Contains("button", StringComparison.Ordinal)).Kind);
        Assert.Equal(SyntaxTokenKind.TypeName, KindOf(tokens, "Button"));
        Assert.Equal(SyntaxTokenKind.Method, KindOf(tokens, "Focus"));
        Assert.Equal(SyntaxTokenKind.Comment, KindOf(tokens, "// hi"));
    }

    [Fact]
    public void CSharpTypesStringsNumbersAndDirectivesAreClassified()
    {
        var tokens = SyntaxHighlighter.Tokenize("#if DEBUG\nColor c = Colors.Red; var n = 42; var s = \"a\\\"b\";", CodeLanguage.CSharp);

        Assert.Equal(SyntaxTokenKind.Preprocessor, KindOf(tokens, "#if DEBUG"));
        Assert.Equal(SyntaxTokenKind.TypeName, KindOf(tokens, "Color"));
        Assert.Equal(SyntaxTokenKind.Number, KindOf(tokens, "42"));
        Assert.Equal(SyntaxTokenKind.StringLiteral, KindOf(tokens, "\"a\\\"b\""));
    }

    [Fact]
    public void AdjacentTokensOfSameKindAreMerged()
    {
        var tokens = SyntaxHighlighter.Tokenize("();", CodeLanguage.CSharp);

        Assert.Single(tokens);
    }

    [Fact]
    public void RawStringsDoNotLeakComments()
    {
        var source = "var s = \"\"\"Before \"//\" after\"\"\"; var next = 1;";
        var tokens = SyntaxHighlighter.Tokenize(source, CodeLanguage.CSharp);

        Assert.Equal(source, string.Concat(tokens.Select(t => t.Text)));
        Assert.DoesNotContain(tokens, t => t.Kind == SyntaxTokenKind.Comment);
        Assert.Equal(SyntaxTokenKind.Number, KindOf(tokens, "1"));
    }
}
