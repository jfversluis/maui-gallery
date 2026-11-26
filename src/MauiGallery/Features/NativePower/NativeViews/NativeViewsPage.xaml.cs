using System.Globalization;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.NativePower;

public sealed partial class NativeViewsPage : SamplePage
{
    public NativeViewsPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        CalendarExample.NativeTarget = CalendarSample;
        PlatformOnlyExample.NativeTarget = PlatformOnlySample;
        CreatePlatformOnlyView();
    }

    private void OnCalendarDateSelected(object? sender, DateChangedEventArgs e) =>
        CalendarExample.Output = $"DateSelected: {e.NewDate:D}";

    private void OnCalendarLimitToggled(object? sender, ToggledEventArgs e)
    {
        CalendarSample.MinimumDate = e.Value ? DateTime.Today : null;
        CalendarSample.MaximumDate = e.Value ? DateTime.Today.AddDays(30) : null;
        CalendarMinimum.Value = CalendarSample.MinimumDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        CalendarMaximum.Value = CalendarSample.MaximumDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }

    private void OnAnimateToTodayClicked(object? sender, EventArgs e) => CalendarSample.AnimateTo(DateTime.Today);

    private void OnAnimateToLaterClicked(object? sender, EventArgs e) => CalendarSample.AnimateTo(DateTime.Today.AddDays(21));

    // Each platform hosts a control that only it has. The #if blocks keep the native types out of the other builds.
    private void CreatePlatformOnlyView()
    {
#if IOS || MACCATALYST
        PlatformOnlyCaption.Text = "Highlight color";
        PlatformOnlySample.WidthRequest = 44;
        PlatformOnlySample.HeightRequest = 44;
        PlatformOnlySample.Create = _ =>
        {
            var well = new UIKit.UIColorWell { Title = "Highlight color", SupportsAlpha = false, SelectedColor = UIKit.UIColor.SystemPink };
            well.ValueChanged += OnColorWellChanged;
            return well;
        };
        PlatformOnlySample.Release = view => ((UIKit.UIColorWell)view).ValueChanged -= OnColorWellChanged;
#elif ANDROID
        PlatformOnlyCaption.Text = "Show only";
        PlatformOnlySample.Create = mauiContext =>
        {
            var context = mauiContext.Context ?? throw new InvalidOperationException("The view has no Android context.");
            var chip = new Google.Android.Material.Chip.Chip(context);
            chip.SetChipDrawable(Google.Android.Material.Chip.ChipDrawable.CreateFromAttributes(
                context, null, 0, Resource.Style.Widget_MaterialComponents_Chip_Filter));
            chip.Text = "Vegetarian";
            chip.CheckedChange += OnChipCheckedChange;
            return chip;
        };
        PlatformOnlySample.Release = view => ((Google.Android.Material.Chip.Chip)view).CheckedChange -= OnChipCheckedChange;
#elif WINDOWS
        PlatformOnlyCaption.Text = "Your rating";
        PlatformOnlySample.Create = _ =>
        {
            var rating = new Microsoft.UI.Xaml.Controls.RatingControl { Value = 3 };
            rating.ValueChanged += OnRatingValueChanged;
            return rating;
        };
        PlatformOnlySample.Release = view => ((Microsoft.UI.Xaml.Controls.RatingControl)view).ValueChanged -= OnRatingValueChanged;
#endif
    }

#if IOS || MACCATALYST
    private void OnColorWellChanged(object? sender, EventArgs e)
    {
        if (sender is UIKit.UIColorWell { SelectedColor: { } color })
        {
            PlatformOnlyExample.Output = $"UIColorWell.SelectedColor: {Microsoft.Maui.Platform.ColorExtensions.ToColor(color)?.ToArgbHex()}";
        }
    }
#elif ANDROID
    private void OnChipCheckedChange(object? sender, Android.Widget.CompoundButton.CheckedChangeEventArgs e) =>
        PlatformOnlyExample.Output = $"Chip.Checked: {e.IsChecked}";
#elif WINDOWS
    private void OnRatingValueChanged(Microsoft.UI.Xaml.Controls.RatingControl sender, object args) =>
        PlatformOnlyExample.Output = string.Create(CultureInfo.InvariantCulture, $"RatingControl.Value: {sender.Value}");
#endif
}
