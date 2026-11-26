using MauiGallery.Core.Shortcuts;

namespace MauiGallery.Core.Tests;

public sealed class ShortcutFormatterTests
{
    [Theory]
    [InlineData(ShortcutModifiers.Cmd, "k", "⌘K")]
    [InlineData(ShortcutModifiers.Cmd | ShortcutModifiers.Shift, "d", "⇧⌘D")]
    [InlineData(ShortcutModifiers.Cmd | ShortcutModifiers.Alt | ShortcutModifiers.Ctrl, "1", "⌃⌥⌘1")]
    [InlineData(ShortcutModifiers.Cmd, ",", "⌘,")]
    [InlineData(ShortcutModifiers.None, "F5", "F5")]
    public void FormatsMacSymbolsInAppleOrder(ShortcutModifiers modifiers, string key, string expected) =>
        Assert.Equal(expected, ShortcutFormatter.Format(modifiers, key, ShortcutStyle.Mac));

    [Theory]
    [InlineData(ShortcutModifiers.Ctrl, "k", "Ctrl+K")]
    [InlineData(ShortcutModifiers.Shift | ShortcutModifiers.Ctrl, "d", "Ctrl+Shift+D")]
    [InlineData(ShortcutModifiers.Windows | ShortcutModifiers.Alt, "x", "Win+Alt+X")]
    [InlineData(ShortcutModifiers.Cmd | ShortcutModifiers.Shift, "d", "Shift+D")]
    public void FormatsWindowsNamesWithPlusSigns(ShortcutModifiers modifiers, string key, string expected) =>
        Assert.Equal(expected, ShortcutFormatter.Format(modifiers, key, ShortcutStyle.Windows));

    [Fact]
    public void ModifiersThePlatformLacksHaveNoName()
    {
        Assert.Null(ShortcutFormatter.ModifierName(ShortcutModifiers.Cmd, ShortcutStyle.Windows));
        Assert.Null(ShortcutFormatter.ModifierName(ShortcutModifiers.Windows, ShortcutStyle.Mac));
        Assert.Equal("⌥ Option", ShortcutFormatter.ModifierName(ShortcutModifiers.Alt, ShortcutStyle.Mac));
    }
}
