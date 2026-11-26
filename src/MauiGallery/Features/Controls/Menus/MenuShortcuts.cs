using MauiGallery.Core.Shortcuts;

namespace MauiGallery.Features.Controls.Menus;

/// <summary>Helpers the menu samples share: shortcut text for the current platform, and menu bar refreshes.</summary>
internal static class MenuShortcuts
{
    public static ShortcutStyle Style => DeviceInfo.Platform == DevicePlatform.WinUI ? ShortcutStyle.Windows : ShortcutStyle.Mac;

    /// <summary>The primary modifier: Command on Mac, Control everywhere else.</summary>
    public static KeyboardAcceleratorModifiers Primary =>
        DeviceInfo.Platform == DevicePlatform.MacCatalyst || DeviceInfo.Platform == DevicePlatform.iOS
            ? KeyboardAcceleratorModifiers.Cmd
            : KeyboardAcceleratorModifiers.Ctrl;

    public static string Format(KeyboardAccelerator accelerator) =>
        ShortcutFormatter.Format((ShortcutModifiers)accelerator.Modifiers, accelerator.Key ?? string.Empty, Style);

    public static string Format(KeyboardAcceleratorModifiers modifiers, string key) =>
        ShortcutFormatter.Format((ShortcutModifiers)modifiers, key, Style);

    /// <summary>
    /// Mac Catalyst builds its menus once and only rebuilds them when the menu bar's items change, so a new Text or
    /// KeyboardAccelerator on an existing item doesn't show up. Re-adding the menu triggers a rebuild.
    /// </summary>
    public static void Refresh(this Page page, MenuBarItem menu)
    {
        var index = page.MenuBarItems.IndexOf(menu);
        if (index < 0)
        {
            return;
        }

        page.MenuBarItems.RemoveAt(index);
        page.MenuBarItems.Insert(index, menu);
    }
}
