using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.StatusAndInfo;

public sealed partial class ActivityIndicatorPage : SamplePage
{
    public ActivityIndicatorPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        LoadSample.BindingContext = new ActivityIndicatorForecastViewModel();
        LoadExample.NativeTarget = LoadIndicator;
    }
}
