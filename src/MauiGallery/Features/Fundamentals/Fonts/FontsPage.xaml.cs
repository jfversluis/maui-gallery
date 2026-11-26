using IconFont.Maui.FluentIcons;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class FontsPage : SamplePage
{
    private static readonly Dictionary<string, string> Glyphs = new()
    {
        [nameof(FluentIconsRegular.Heart24)] = FluentIconsRegular.Heart24,
        [nameof(FluentIconsRegular.Star24)] = FluentIconsRegular.Star24,
        [nameof(FluentIconsRegular.Home24)] = FluentIconsRegular.Home24,
        [nameof(FluentIconsRegular.Settings24)] = FluentIconsRegular.Settings24,
        [nameof(FluentIconsRegular.Camera24)] = FluentIconsRegular.Camera24,
        [nameof(FluentIconsRegular.Mail24)] = FluentIconsRegular.Mail24,
    };

    public FontsPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        IconsExample.NativeTarget = IconImage;
        OnFamilyChanged(this, EventArgs.Empty);
        OnAttributesChanged(this, EventArgs.Empty);
        OnGlyphChanged(this, EventArgs.Empty);
    }

    private void OnFamilyChanged(object? sender, EventArgs e)
    {
        // The first option leaves FontFamily unset, which uses the platform's system font.
        var family = FamilyPicker.SelectedIndex > 0 ? FamilyPicker.SelectedItem as string : null;
        FamilySample.FontFamily = family;
        FamilyText.Value = family;
    }

    private void OnAttributesChanged(object? sender, EventArgs e)
    {
        var attributes = Enum.Parse<FontAttributes>(AttributesPicker.SelectedItem as string ?? nameof(FontAttributes.None));
        SizeSample.FontAttributes = attributes;
        AttributesXaml.Value = AttributesPicker.SelectedItem;
        AttributesCode.Value = attributes switch
        {
            FontAttributes.Bold | FontAttributes.Italic => "FontAttributes.Bold | FontAttributes.Italic",
            _ => $"FontAttributes.{attributes}",
        };
    }

    private void OnGlyphChanged(object? sender, EventArgs e)
    {
        var glyph = Glyphs[GlyphPicker.SelectedItem as string ?? nameof(FluentIconsRegular.Heart24)];
        ImageIcon.Glyph = glyph;
        ButtonIcon.Glyph = glyph;
    }
}
