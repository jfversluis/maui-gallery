using System.Globalization;
using MauiGallery.Features.Samples;
using Microsoft.Maui.Layouts;

namespace MauiGallery.Features.Controls.Layouts;

public sealed partial class FlexLayoutPage : SamplePage
{
    public FlexLayoutPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        ContainerExample.NativeTarget = ContainerSample;
        TagsSample.BindingContext = new TagCloudViewModel();
        TagsExample.NativeTarget = TagsFlex;
        ItemsExample.NativeTarget = ItemsSample;
        OnItemsChanged(this, EventArgs.Empty);
    }

    private void OnItemsChanged(object? sender, EventArgs e)
    {
        var basis = ItemsBasis.SelectedItem as string ?? "Auto";
        FlexLayout.SetBasis(BasisTile, ParseBasis(basis));
        FlexLayout.SetGrow(GrowTile, (float)ItemsGrow.Value);
        FlexLayout.SetShrink(ShrinkTile, (float)ItemsShrink.Value);
        BasisCode.Value = basis switch
        {
            "Auto" => null,
            _ when basis.EndsWith('%') => $"new FlexBasis({float.Parse(basis.TrimEnd('%'), CultureInfo.InvariantCulture) / 100:0.##}f, isRelative: true)",
            _ => basis,
        };
    }

    // XAML accepts "Auto", "50%" or "120"; percentages are relative to the size of the line.
    private static FlexBasis ParseBasis(string value) => value switch
    {
        "Auto" => FlexBasis.Auto,
        _ when value.EndsWith('%') => new FlexBasis(float.Parse(value.TrimEnd('%'), CultureInfo.InvariantCulture) / 100, isRelative: true),
        _ => new FlexBasis(float.Parse(value, CultureInfo.InvariantCulture)),
    };
}
