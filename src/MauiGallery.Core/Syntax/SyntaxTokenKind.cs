namespace MauiGallery.Core.Syntax;

/// <summary>The classification of a highlighted code fragment. Each kind maps to one color in the code viewer.</summary>
public enum SyntaxTokenKind
{
    Plain,
    Punctuation,
    Comment,
    Keyword,
    StringLiteral,
    Number,
    TypeName,
    Method,
    ElementName,
    AttributeName,
    AttributeValue,
    MarkupExtension,
    Preprocessor,
}
