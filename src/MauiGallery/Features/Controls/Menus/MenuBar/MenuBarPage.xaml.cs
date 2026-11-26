using System.Globalization;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Menus;

public sealed partial class MenuBarPage : SamplePage
{
    private const string CheckMark = "✓ ";
    private MenuBarItem? _dynamicMenu;

    public MenuBarPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        BasicHint.Text = $"Open the Sample menu, or press {MenuShortcuts.Format(BoldItem.KeyboardAccelerators[0])} for bold.";
        DynamicTitle.SelectedIndex = 0;
        UpdateChecks();
    }

    private void OnBoldClicked(object? sender, EventArgs e)
    {
        BasicText.FontAttributes ^= FontAttributes.Bold;
        UpdateChecks();
        BasicExample.Output = $"Bold: {(BasicText.FontAttributes.HasFlag(FontAttributes.Bold) ? "on" : "off")}";
    }

    private void OnItalicClicked(object? sender, EventArgs e)
    {
        BasicText.FontAttributes ^= FontAttributes.Italic;
        UpdateChecks();
        BasicExample.Output = $"Italic: {(BasicText.FontAttributes.HasFlag(FontAttributes.Italic) ? "on" : "off")}";
    }

    private void OnSizeClicked(object? sender, EventArgs e)
    {
        if (sender is MenuFlyoutItem { CommandParameter: string size } item)
        {
            BasicText.FontSize = double.Parse(size, CultureInfo.InvariantCulture);
            UpdateChecks();
            BasicExample.Output = $"Text Size > {item.Text.Replace(CheckMark, string.Empty, StringComparison.Ordinal)}";
        }
    }

    private void OnResetClicked(object? sender, EventArgs e)
    {
        BasicText.FontAttributes = FontAttributes.None;
        BasicText.FontSize = 18;
        UpdateChecks();
        BasicExample.Output = "Reset";
    }

    // MAUI menu items have no checked state, so a check mark in the text shows which options are on.
    private void UpdateChecks()
    {
        SetChecked(BoldItem, "Bold", BasicText.FontAttributes.HasFlag(FontAttributes.Bold));
        SetChecked(ItalicItem, "Italic", BasicText.FontAttributes.HasFlag(FontAttributes.Italic));
        SetChecked(SmallItem, "Small", BasicText.FontSize == 14);
        SetChecked(MediumItem, "Medium", BasicText.FontSize == 18);
        SetChecked(LargeItem, "Large", BasicText.FontSize == 24);
        this.Refresh(SampleMenu);
    }

    private static void SetChecked(MenuFlyoutItem item, string text, bool isChecked) =>
        item.Text = isChecked ? CheckMark + text : text;

    private void OnDynamicChanged(object? sender, EventArgs e)
    {
        if (_dynamicMenu is not null)
        {
            MenuBarItems.Remove(_dynamicMenu);
            _dynamicMenu = null;
        }

        var title = (string?)DynamicTitle.SelectedItem ?? "Tools";
        if (!DynamicShow.IsToggled)
        {
            DynamicStatus.Text = "No extra menu: the menu bar shows the app's menus and the Sample menu.";
            return;
        }

        var wordCount = new MenuFlyoutItem { Text = "Word Count" };
        wordCount.Clicked += (_, _) => DynamicExample.Output =
            $"{title} > Word Count: {BasicText.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length} words";
        var characterCount = new MenuFlyoutItem { Text = "Character Count" };
        characterCount.Clicked += (_, _) => DynamicExample.Output = $"{title} > Character Count: {BasicText.Text.Length} characters";

        _dynamicMenu = new MenuBarItem { Text = title };
        _dynamicMenu.Add(wordCount);
        _dynamicMenu.Add(new MenuFlyoutSeparator());
        _dynamicMenu.Add(characterCount);
        MenuBarItems.Add(_dynamicMenu);

        DynamicStatus.Text = title == "Tools"
            ? "Look for the Tools menu in the menu bar."
            : DeviceInfo.Platform == DevicePlatform.MacCatalyst
                ? "Open the Edit menu: Word Count and Character Count are at the top."
                : "Look for the second Edit menu in the menu bar.";
    }
}
