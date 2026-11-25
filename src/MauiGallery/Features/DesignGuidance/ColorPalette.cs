namespace MauiGallery.Features.DesignGuidance;

public sealed record ColorToken(string Name, Color Color)
{
    public string Hex => Color.ToArgbHex(includeAlpha: Color.Alpha < 1);
}

public sealed record ColorTokenGroup(string Title, IReadOnlyList<ColorToken> Tokens);

/// <summary>
/// Reads the design system's color tokens from the active theme. An invisible element in the page, so it is told
/// about theme changes through the resource system and can refresh the swatches without event subscriptions.
/// </summary>
public sealed class ColorPalette : View
{
    public static readonly BindableProperty GroupsProperty =
        BindableProperty.Create(nameof(Groups), typeof(IReadOnlyList<ColorTokenGroup>), typeof(ColorPalette), Array.Empty<ColorTokenGroup>());

    private static readonly (string Title, string[] Keys)[] TokenGroups =
    [
        ("Surfaces", ["ColorBackground", "ColorSurface", "ColorSurfaceAlt", "ColorSidebar", "ColorOutline", "ColorOutlineStrong"]),
        ("Text", ["ColorTextPrimary", "ColorTextSecondary", "ColorTextTertiary"]),
        ("Accent", ["ColorAccent", "ColorAccentHover", "ColorAccentSubtle", "ColorAccentText", "ColorOnAccent", "ColorSelection"]),
        ("Status", ["ColorSuccess", "ColorWarning", "ColorError", "ColorInfo"]),
        ("Stage and code", ["ColorStage", "ColorStageDot", "ColorCodeBackground"]),
    ];

    // Any token works as the change signal: all of them are swapped together.
    private static readonly BindableProperty ThemeSignalProperty =
        BindableProperty.Create("ThemeSignal", typeof(Color), typeof(ColorPalette), propertyChanged: (b, _, _) => ((ColorPalette)b).Refresh());

    public ColorPalette() => SetDynamicResource(ThemeSignalProperty, "ColorBackground");

    public IReadOnlyList<ColorTokenGroup> Groups
    {
        get => (IReadOnlyList<ColorTokenGroup>)GetValue(GroupsProperty);
        private set => SetValue(GroupsProperty, value);
    }

    private void Refresh()
    {
        var resources = Application.Current?.Resources;
        if (resources is null)
        {
            return;
        }

        Groups = [.. TokenGroups.Select(group => new ColorTokenGroup(
            group.Title,
            [.. group.Keys.Where(resources.ContainsKey).Select(key => new ColorToken(key["Color".Length..], (Color)resources[key]))]))];
    }
}
