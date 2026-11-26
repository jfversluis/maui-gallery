using Microsoft.Maui.Handlers;

namespace MauiGallery.Features.NativePower;

/// <summary>
/// A <see cref="Slider"/> that plays the platform's selection haptic each time its value crosses a multiple of
/// <see cref="Step"/>, like the detents of a picker wheel.
/// </summary>
public sealed class HapticSlider : Slider
{
    public static readonly BindableProperty StepProperty =
        BindableProperty.Create(nameof(Step), typeof(double), typeof(HapticSlider), 10d);

#if IOS || MACCATALYST
    private static UIKit.UISelectionFeedbackGenerator? s_feedback;
#endif

    private int? _detent;

    /// <summary>Raised after a haptic tick, with the step value the slider crossed.</summary>
    public event EventHandler<double>? Ticked;

    public double Step
    {
        get => (double)GetValue(StepProperty);
        set => SetValue(StepProperty, value);
    }

    internal static void Customize() =>
        SliderHandler.Mapper.ModifyMapping(nameof(ISlider.Value), (handler, view, mapValue) =>
        {
            // Let .NET MAUI move the native thumb first, then add to it.
            mapValue?.Invoke(handler, view);

            if (view is not HapticSlider slider || !slider.CrossedDetent(out var detent))
            {
                return;
            }

#if IOS || MACCATALYST
            s_feedback ??= new UIKit.UISelectionFeedbackGenerator();
            s_feedback.SelectionChanged();
#elif ANDROID
            handler.PlatformView.PerformHapticFeedback(OperatingSystem.IsAndroidVersionAtLeast(27)
                ? Android.Views.FeedbackConstants.ClockTick
                : Android.Views.FeedbackConstants.KeyboardTap);
#endif
            // Windows has no haptics for sliders, so it only raises the event.
            slider.Ticked?.Invoke(slider, detent);
        });

    private bool CrossedDetent(out double detent)
    {
        var step = Step > 0 ? Step : 1;
        var index = (int)Math.Round(Value / step);
        detent = index * step;

        // The first mapping runs when the handler connects; that isn't a change.
        var crossed = _detent is not null && _detent != index;
        _detent = index;
        return crossed;
    }
}
