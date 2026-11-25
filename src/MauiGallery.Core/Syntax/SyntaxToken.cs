namespace MauiGallery.Core.Syntax;

/// <summary>A contiguous fragment of source code and its classification.</summary>
public readonly record struct SyntaxToken(SyntaxTokenKind Kind, string Text);
