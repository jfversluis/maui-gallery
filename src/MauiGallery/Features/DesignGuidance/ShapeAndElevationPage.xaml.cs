using MauiGallery.Features.Samples;

namespace MauiGallery.Features.DesignGuidance;

public sealed partial class ShapeAndElevationPage : SamplePage
{
    public ShapeAndElevationPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        OnShadowChanged(this, EventArgs.Empty);
    }

    private void OnShadowChanged(object? sender, EventArgs e)
    {
        // Options raise their events while the page is still loading.
        if (CardSample is null || CardShadow?.SelectedItem is not string shadow)
        {
            return;
        }

        if (shadow == "None")
        {
            CardSample.ClearValue(VisualElement.ShadowProperty);
        }
        else
        {
            // A dynamic resource, so the shadow follows the theme.
            CardSample.SetDynamicResource(VisualElement.ShadowProperty, shadow);
        }

        CardExample.Output = $"Shadow: {shadow}";
    }

    private void OnOutlineToggled(object? sender, ToggledEventArgs e)
    {
        CardSample.StrokeThickness = e.Value ? 1 : 0;
        CardOutlineValue.Value = CardSample.StrokeThickness;
    }
}
