using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Layouts;

public sealed partial class CustomLayoutPage : SamplePage
{
    public CustomLayoutPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        CircularExample.NativeTarget = CircularSample;
        WrapExample.NativeTarget = WrapSample;
        OnCircularCountChanged(this, EventArgs.Empty);
    }

    private void OnCircularCountChanged(object? sender, EventArgs e) =>
        BindableLayout.SetItemsSource(CircularSample, Enumerable.Range(1, (int)CircularCount.Value).ToArray());
}
