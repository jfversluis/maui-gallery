using MauiGallery.Core.Snippets;

namespace MauiGallery.DesignSystem.Components;

/// <summary>
/// Connects a <c>$(Key)</c> placeholder in a code snippet to a live option value. When <see cref="Value"/> equals
/// <see cref="Default"/>, the snippet line containing the placeholder is omitted, so the code only shows what changed.
/// </summary>
public sealed class Substitution : BindableObject
{
    public static readonly BindableProperty KeyProperty =
        BindableProperty.Create(nameof(Key), typeof(string), typeof(Substitution), string.Empty);

    public static readonly BindableProperty ValueProperty =
        BindableProperty.Create(nameof(Value), typeof(object), typeof(Substitution));

    public static readonly BindableProperty DefaultProperty =
        BindableProperty.Create(nameof(Default), typeof(object), typeof(Substitution));

    public static readonly BindableProperty IsExpressionProperty =
        BindableProperty.Create(nameof(IsExpression), typeof(bool), typeof(Substitution), false);

    public string Key
    {
        get => (string)GetValue(KeyProperty);
        set => SetValue(KeyProperty, value);
    }

    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>The control's default value. Compared as text, so <c>Default="True"</c> matches a <see langword="bool"/>.</summary>
    public object? Default
    {
        get => GetValue(DefaultProperty);
        set => SetValue(DefaultProperty, value);
    }

    /// <summary>
    /// When <see langword="true"/>, <see cref="Value"/> is C# source (such as an enum member) and is inserted into
    /// C# snippets as-is instead of as a string literal.
    /// </summary>
    public bool IsExpression
    {
        get => (bool)GetValue(IsExpressionProperty);
        set => SetValue(IsExpressionProperty, value);
    }

    /// <summary>The value as it appears in XAML, or <see langword="null"/> when it is the default.</summary>
    public string? XamlText => IsDefault ? null : Value is Color color ? ToHex(color) : SnippetValueFormatter.Format(Value);

    /// <summary>The value as a C# literal, or <see langword="null"/> when it is the default.</summary>
    public string? CSharpText => IsDefault ? null : Value switch
    {
        Color color => $"Color.FromArgb(\"{ToHex(color)}\")",
        _ when IsExpression => SnippetValueFormatter.Format(Value),
        _ => SnippetValueFormatter.FormatCSharp(Value),
    };

    private static string ToHex(Color color) => color.ToArgbHex(includeAlpha: color.Alpha < 1);

    private bool IsDefault =>
        Default is not null && SnippetValueFormatter.Format(Default) == SnippetValueFormatter.Format(Value);
}
