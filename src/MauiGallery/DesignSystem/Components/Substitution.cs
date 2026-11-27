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

    /// <summary>The value as it appears in XAML, or <see langword="null"/> (omit the line) when it is the default or unset.</summary>
    public string? XamlText => IsDefault || Value is null ? null : Value is Color color ? ToHex(color) : SnippetValueFormatter.FormatXamlAttribute(Value);

    /// <summary>The value as a C# literal, or <see langword="null"/> when it is the default.</summary>
    public string? CSharpText => IsDefault || Value is null ? null : Value switch
    {
        Color color => $"Color.FromArgb(\"{ToHex(color)}\")",
        _ when IsExpression => SnippetValueFormatter.Format(Value),
        _ => SnippetValueFormatter.FormatCSharp(Value),
    };

    private static string ToHex(Color color) => color.ToArgbHex(includeAlpha: color.Alpha < 1);

    // Case matters: Format="D" is not the default Format="d".
    private bool IsDefault =>
        Default is not null && string.Equals(Normalize(Default), Normalize(Value), StringComparison.Ordinal);

    // Compares as source text, so Default="True" matches true and Default="#512BD4" matches a Color. Other text is
    // case-sensitive: a "D" (long date) format isn't the default "d".
    private static string Normalize(object? value) => value switch
    {
        Color color => ToHex(color),
        string text when bool.TryParse(text, out var flag) => SnippetValueFormatter.Format(flag),
        string text when Color.TryParse(text, out var color) => ToHex(color),
        string text when bool.TryParse(text, out var flag) => SnippetValueFormatter.Format(flag),
        string text when double.TryParse(text, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var number) => SnippetValueFormatter.Format(number),
        _ => SnippetValueFormatter.Format(value),
    };
}
