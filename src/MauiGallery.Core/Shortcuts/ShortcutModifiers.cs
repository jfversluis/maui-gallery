namespace MauiGallery.Core.Shortcuts;

/// <summary>
/// Keyboard shortcut modifiers. The values match MAUI's <c>KeyboardAcceleratorModifiers</c>, so the app can cast
/// between the two.
/// </summary>
[Flags]
public enum ShortcutModifiers
{
    None = 0,
    Shift = 1 << 0,
    Ctrl = 1 << 1,
    Alt = 1 << 2,
    Cmd = 1 << 3,
    Windows = 1 << 4,
}
