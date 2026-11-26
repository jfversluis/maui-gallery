using MauiGallery.Core.Shortcuts;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Menus;

public sealed partial class KeyboardAcceleratorsPage : SamplePage
{
    private static readonly KeyboardAcceleratorModifiers[] AllModifiers =
    [
        KeyboardAcceleratorModifiers.Cmd,
        KeyboardAcceleratorModifiers.Ctrl,
        KeyboardAcceleratorModifiers.Alt,
        KeyboardAcceleratorModifiers.Shift,
        KeyboardAcceleratorModifiers.Windows,
    ];

    private int _duplicates;

    public KeyboardAcceleratorsPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        if (MenuShortcuts.Style == ShortcutStyle.Mac)
        {
            BasicCtrlLabel.Text = "Ctrl (⌃ Control)";
            BasicAltLabel.Text = "Alt (⌥ Option)";
            BasicShiftLabel.Text = "Shift (⇧)";
        }

        BasicCmd.IsChecked = MenuShortcuts.Primary == KeyboardAcceleratorModifiers.Cmd;
        BasicCtrl.IsChecked = MenuShortcuts.Primary == KeyboardAcceleratorModifiers.Ctrl;
        BasicKey.SelectedIndex = 0;
        BuildPlatformTable();
        Loaded += OnLoaded;
    }

    // The page has a Window once it's loaded; the menu bar belongs to that window's Shell.
    private void OnLoaded(object? sender, EventArgs e)
    {
        if (Window?.Page is Shell shell)
        {
            BindableLayout.SetItemsSource(AppShortcuts, shell.MenuBarItems.SelectMany(menu => ShortcutsIn(menu, menu.Text)).ToList());
        }
    }

    private static IEnumerable<ShortcutRow> ShortcutsIn(IEnumerable<IMenuElement> items, string prefix) =>
        items.SelectMany(item => item switch
        {
            MenuFlyoutSubItem submenu => ShortcutsIn(submenu, $"{prefix} > {submenu.Text}"),
            MenuFlyoutItem { KeyboardAccelerators: [var shortcut, ..] } menuItem =>
                [new ShortcutRow($"{prefix} > {menuItem.Text}", MenuShortcuts.Format(shortcut))],
            _ => [],
        });

    private void OnDuplicateClicked(object? sender, EventArgs e)
    {
        _duplicates++;
        BasicCount.Text = _duplicates == 1 ? "Duplicated once" : $"Duplicated {_duplicates} times";
        BasicExample.Output = $"Shortcuts > Duplicate ({MenuShortcuts.Format(DuplicateItem.KeyboardAccelerators[0])})";
    }

    private void OnShortcutChanged(object? sender, EventArgs e)
    {
        if (BasicKey?.SelectedItem is not string key || BasicShift is null)
        {
            return;
        }

        var modifiers = KeyboardAcceleratorModifiers.None;
        modifiers |= BasicCmd.IsChecked && BasicCmdRow.IsVisible ? KeyboardAcceleratorModifiers.Cmd : 0;
        modifiers |= BasicCtrl.IsChecked ? KeyboardAcceleratorModifiers.Ctrl : 0;
        modifiers |= BasicAlt.IsChecked ? KeyboardAcceleratorModifiers.Alt : 0;
        modifiers |= BasicShift.IsChecked ? KeyboardAcceleratorModifiers.Shift : 0;

        DuplicateItem.KeyboardAccelerators.Clear();
        DuplicateItem.KeyboardAccelerators.Add(new KeyboardAccelerator { Modifiers = modifiers, Key = key });
        this.Refresh(ShortcutsMenu);

        BasicHint.Text = $"Press {MenuShortcuts.Format(modifiers, key)}, or choose Shortcuts > Duplicate.";

        var names = AllModifiers.Where(m => modifiers.HasFlag(m)).ToList();
        BasicModifiersSubstitution.Value = names.Count == 0 ? "None" : string.Join(",", names);
        BasicModifiersCodeSubstitution.Value = names.Count == 0
            ? "KeyboardAcceleratorModifiers.None"
            : string.Join(" | ", names.Select(m => $"KeyboardAcceleratorModifiers.{m}"));
    }

    private void BuildPlatformTable()
    {
        AddRow(0, "Modifier", "Mac", "Windows", isHeader: true);
        var row = 1;
        foreach (var modifier in AllModifiers)
        {
            var shortcut = (ShortcutModifiers)modifier;
            AddRow(
                row++,
                modifier.ToString(),
                ShortcutFormatter.ModifierName(shortcut, ShortcutStyle.Mac) ?? "–",
                ShortcutFormatter.ModifierName(shortcut, ShortcutStyle.Windows) ?? "–",
                isHeader: false);
        }

        void AddRow(int index, string name, string mac, string windows, bool isHeader)
        {
            PlatformTable.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            var attributes = isHeader ? FontAttributes.Bold : FontAttributes.None;
            PlatformTable.Add(new Label { Text = name, FontAttributes = attributes }, 0, index);
            PlatformTable.Add(new Label { Text = mac, FontAttributes = attributes }, 1, index);
            PlatformTable.Add(new Label { Text = windows, FontAttributes = attributes }, 2, index);
        }
    }
}
