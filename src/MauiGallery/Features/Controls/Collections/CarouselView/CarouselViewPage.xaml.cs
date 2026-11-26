using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Collections;

public sealed partial class CarouselViewPage : SamplePage
{
    public CarouselViewPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        PositionSample.BindingContext = new TripCarouselViewModel();
        PositionExample.NativeTarget = PositionCarousel;
        IndicatorExample.NativeTarget = IndicatorCarousel;
    }

    private void OnBasicCurrentItemChanged(object? sender, CurrentItemChangedEventArgs e)
    {
        if (e.CurrentItem is Trip trip)
        {
            BasicExample.Output = $"CurrentItemChanged: {trip.Title} (Position {BasicCarousel.Position})";
        }
    }

    private void OnBasicLoopToggled(object? sender, ToggledEventArgs e)
    {
        BasicCarousel.Loop = e.Value;

        // Workaround: on iOS and Mac Catalyst the carousel keeps showing the looped neighbors after Loop changes at
        // runtime, so reload the items. Apps rarely change Loop after the carousel is shown.
        var position = BasicCarousel.Position;
        BasicCarousel.ItemsSource = null;
        BasicCarousel.ItemsSource = TripData.All;
        BasicCarousel.Position = position;
    }

    private void OnBasicPeekChanged(object? sender, ValueChangedEventArgs e) =>
        BasicCarousel.PeekAreaInsets = new Thickness(Math.Round(e.NewValue), 0);
}
