using System.ComponentModel;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Layouts;

public sealed partial class ContentViewPage : SamplePage
{
    public ContentViewPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        CardExample.NativeTarget = FirstCard;
        RatingExample.NativeTarget = Rating;
        SectionExample.NativeTarget = Section;
        UpdateRatingSummary();
        Rating.PropertyChanged += OnRatingPropertyChanged;
    }

    private void OnRatingPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(RatingView.Value) or nameof(RatingView.Maximum))
        {
            UpdateRatingSummary();
        }

        if (e.PropertyName == nameof(RatingView.Value))
        {
            RatingExample.Output = $"Value changed to {Rating.Value}";
        }
    }

    private void UpdateRatingSummary() => RatingSummary.Text = $"{Rating.Value} of {Rating.Maximum}";
}
