using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Layouts;

public sealed partial class StackLayoutPage : SamplePage
{
    public StackLayoutPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        VerticalExample.NativeTarget = VerticalSample;
        HorizontalExample.NativeTarget = HorizontalSample;
        NestedExample.NativeTarget = NestedSample;
        LegacyExample.NativeTarget = LegacySample;
        OnVerticalChildOptionsChanged(this, EventArgs.Empty);
        OnHorizontalChildOptionsChanged(this, EventArgs.Empty);
    }

    private void OnVerticalChildOptionsChanged(object? sender, EventArgs e)
    {
        var options = ToLayoutOptions(VerticalChildOptions.SelectedItem);
        foreach (var child in VerticalSample.Children.OfType<View>())
        {
            child.HorizontalOptions = options;
        }
    }

    private void OnHorizontalChildOptionsChanged(object? sender, EventArgs e)
    {
        var options = ToLayoutOptions(HorizontalChildOptions.SelectedItem);

        // The tall tile sets the stack's height, so only the shorter ones move.
        foreach (var child in HorizontalSample.Children.OfType<View>().Where(c => c.HeightRequest is < 96))
        {
            child.VerticalOptions = options;
        }
    }

    internal static LayoutOptions ToLayoutOptions(object? name) => name switch
    {
        "Start" => LayoutOptions.Start,
        "Center" => LayoutOptions.Center,
        "End" => LayoutOptions.End,
        _ => LayoutOptions.Fill,
    };
}
