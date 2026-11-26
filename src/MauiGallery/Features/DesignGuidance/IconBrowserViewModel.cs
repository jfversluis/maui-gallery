using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using IconFont.Maui.FluentIcons;

namespace MauiGallery.Features.DesignGuidance;

/// <summary>One tile in the icon browser: the glyph in the chosen style and the constant that names it.</summary>
public sealed record IconTile(string Name, string Glyph, string FontFamily, string Constant);

/// <summary>Filters the curated icon set and copies an icon's constant name, ready to paste into XAML or C#.</summary>
public sealed partial class IconBrowserViewModel(IClipboard clipboard) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Icons), nameof(Summary))]
    public partial string? SearchText { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Icons))]
    public partial bool IsFilled { get; set; }

    [ObservableProperty]
    public partial string? Status { get; set; }

    public IReadOnlyList<IconTile> Icons =>
    [
        .. FluentIconSet.Common
            .Where(icon => string.IsNullOrWhiteSpace(SearchText) || icon.Name.Contains(SearchText.Trim(), StringComparison.OrdinalIgnoreCase))
            .Select(icon => IsFilled
                ? new IconTile(icon.Name, icon.Filled, FluentIconsFilled.FontFamily, $"FluentIconsFilled.{icon.Name}24")
                : new IconTile(icon.Name, icon.Regular, FluentIconsRegular.FontFamily, $"FluentIconsRegular.{icon.Name}24")),
    ];

    public string Summary => Icons.Count switch
    {
        0 => $"No icons match \"{SearchText?.Trim()}\" in this set. The package has thousands more: try its IntelliSense.",
        var count => $"{count} of {FluentIconSet.Common.Count} icons. Tap one to copy its name.",
    };

    [RelayCommand]
    private async Task CopyAsync(IconTile icon)
    {
        await clipboard.SetTextAsync(icon.Constant);
        Status = $"Copied {icon.Constant}";
        SemanticScreenReader.Announce(Status);
    }
}
