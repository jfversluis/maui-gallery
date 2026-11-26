using MauiGallery.DesignSystem.Components;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class StylesAndResourcesPage : SamplePage
{
    public StylesAndResourcesPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        ResourceColor.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SwatchPicker.SelectedColor) && ResourceColor.SelectedColor is { } color)
            {
                // Replacing the value updates every DynamicResource that uses the key; StaticResources keep the old one.
                ResourceSample.Resources["Highlight"] = color;
                ResourceExample.Output = $"Resources[\"Highlight\"] = {color.ToArgbHex()}";
            }
        };
    }

    private void OnStyleClassChanged(object? sender, CheckedChangedEventArgs e)
    {
        string[] classes =
        [
            .. ClassTag.IsChecked ? ["Tag"] : Array.Empty<string>(),
            .. ClassAccent.IsChecked ? ["Accent"] : Array.Empty<string>(),
            .. ClassLarge.IsChecked ? ["Large"] : Array.Empty<string>(),
        ];
        ClassLabel.StyleClass = classes;
        ClassSubstitution.Value = string.Join(',', classes);
        ClassExample.Output = $"StyleClass = [{string.Join(", ", classes)}]";
    }
}
