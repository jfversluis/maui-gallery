using System.Text;

namespace MauiGallery.Core.Shortcuts;

/// <summary>The conventions a platform uses to write keyboard shortcuts.</summary>
public enum ShortcutStyle
{
    /// <summary>Symbols without separators, in Apple's order: ⌃⌥⇧⌘K.</summary>
    Mac,

    /// <summary>Names joined with plus signs: Ctrl+Shift+K.</summary>
    Windows,
}

/// <summary>Formats keyboard shortcuts the way each platform shows them in menus.</summary>
public static class ShortcutFormatter
{
    public static string Format(ShortcutModifiers modifiers, string key, ShortcutStyle style)
    {
        ArgumentNullException.ThrowIfNull(key);

        var displayKey = key.Length == 1 ? key.ToUpperInvariant() : key;
        return style == ShortcutStyle.Mac ? FormatMac(modifiers, displayKey) : FormatWindows(modifiers, displayKey);
    }

    /// <summary>The name of a single modifier, such as ⌘ Command or Ctrl, or null when the platform has no such key.</summary>
    public static string? ModifierName(ShortcutModifiers modifier, ShortcutStyle style) => (modifier, style) switch
    {
        (ShortcutModifiers.Shift, ShortcutStyle.Mac) => "⇧ Shift",
        (ShortcutModifiers.Ctrl, ShortcutStyle.Mac) => "⌃ Control",
        (ShortcutModifiers.Alt, ShortcutStyle.Mac) => "⌥ Option",
        (ShortcutModifiers.Cmd, ShortcutStyle.Mac) => "⌘ Command",
        (ShortcutModifiers.Shift, ShortcutStyle.Windows) => "Shift",
        (ShortcutModifiers.Ctrl, ShortcutStyle.Windows) => "Ctrl",
        (ShortcutModifiers.Alt, ShortcutStyle.Windows) => "Alt",
        (ShortcutModifiers.Windows, ShortcutStyle.Windows) => "⊞ Win",
        _ => null,
    };

    private static string FormatMac(ShortcutModifiers modifiers, string key)
    {
        var builder = new StringBuilder();
        Append(ShortcutModifiers.Ctrl, "⌃");
        Append(ShortcutModifiers.Alt, "⌥");
        Append(ShortcutModifiers.Shift, "⇧");
        Append(ShortcutModifiers.Cmd, "⌘");
        return builder.Append(key).ToString();

        void Append(ShortcutModifiers modifier, string symbol)
        {
            if (modifiers.HasFlag(modifier))
            {
                builder.Append(symbol);
            }
        }
    }

    // Windows has no Command key, so Cmd is left out.
    private static string FormatWindows(ShortcutModifiers modifiers, string key)
    {
        var parts = new List<string>(5);
        Append(ShortcutModifiers.Windows, "Win");
        Append(ShortcutModifiers.Ctrl, "Ctrl");
        Append(ShortcutModifiers.Alt, "Alt");
        Append(ShortcutModifiers.Shift, "Shift");
        parts.Add(key);
        return string.Join('+', parts);

        void Append(ShortcutModifiers modifier, string name)
        {
            if (modifiers.HasFlag(modifier))
            {
                parts.Add(name);
            }
        }
    }
}
