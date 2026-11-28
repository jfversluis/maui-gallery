using System.ComponentModel;
using System.Globalization;
using MauiGallery.Features.Samples;
using Microsoft.Maui.Controls.Foldable;
using Microsoft.Maui.Foldable;

namespace MauiGallery.Features.Controls.Layouts;

public sealed partial class TwoPaneViewPage : SamplePage
{
    private const string NotFoldableNote =
        "TwoPaneView follows the hinge on Android foldables. On this device it splits by size alone, and DualScreenInfo reports a single pane.";

    private static readonly TimeSpan HingeAngleTimeout = TimeSpan.FromSeconds(2);

    public TwoPaneViewPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        WideExample.NativeTarget = WideTwoPaneView;
        TallExample.NativeTarget = TallTwoPaneView;

        WideConfiguration.ItemsSource = Enum.GetValues<TwoPaneViewWideModeConfiguration>();
        WideConfiguration.SelectedItem = TwoPaneViewWideModeConfiguration.LeftRight;
        WidePriority.ItemsSource = Enum.GetValues<TwoPaneViewPriority>();
        WidePriority.SelectedItem = TwoPaneViewPriority.Pane1;
        TallConfiguration.ItemsSource = Enum.GetValues<TwoPaneViewTallModeConfiguration>();
        TallConfiguration.SelectedItem = TwoPaneViewTallModeConfiguration.TopBottom;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        DualScreenInfo.Current.PropertyChanged += OnDualScreenInfoChanged;
        ShowDualScreenInfo();
        HingeExample.Notice = DeviceInfo.Platform == DevicePlatform.Android ? null : NotFoldableNote;
    }

    protected override void OnDisappearing()
    {
        DualScreenInfo.Current.PropertyChanged -= OnDualScreenInfoChanged;
        base.OnDisappearing();
    }

    private void OnWideOptionsChanged(object? sender, EventArgs e)
    {
        if (WideConfiguration.SelectedItem is TwoPaneViewWideModeConfiguration configuration)
        {
            WideTwoPaneView.WideModeConfiguration = configuration;
            WideConfigurationSubstitution.Value = configuration.ToString();
            WideConfigurationCode.Value = $"{nameof(TwoPaneViewWideModeConfiguration)}.{configuration}";
        }

        if (WidePriority.SelectedItem is TwoPaneViewPriority priority)
        {
            WideTwoPaneView.PanePriority = priority;
            WidePrioritySubstitution.Value = priority.ToString();
            WidePriorityCode.Value = $"{nameof(TwoPaneViewPriority)}.{priority}";
        }

        if (WidePane1Length.SelectedItem is string length)
        {
            // The presets are "*", "2*" and "120": star shares, or a fixed size.
            (WideTwoPaneView.Pane1Length, WidePane1LengthCode.Value) = length switch
            {
                "*" => (GridLength.Star, "GridLength.Star"),
                _ when length.EndsWith('*') => (
                    new GridLength(double.Parse(length.TrimEnd('*'), CultureInfo.InvariantCulture), GridUnitType.Star),
                    $"new GridLength({length.TrimEnd('*')}, GridUnitType.Star)"),
                _ => (new GridLength(double.Parse(length, CultureInfo.InvariantCulture)), $"new GridLength({length})"),
            };
        }
    }

    private void OnTallOptionsChanged(object? sender, EventArgs e)
    {
        if (TallConfiguration.SelectedItem is TwoPaneViewTallModeConfiguration configuration)
        {
            TallTwoPaneView.TallModeConfiguration = configuration;
            TallConfigurationSubstitution.Value = configuration.ToString();
            TallConfigurationCode.Value = $"{nameof(TwoPaneViewTallModeConfiguration)}.{configuration}";
        }
    }

    private void OnWideModeChanged(object? sender, EventArgs e) =>
        WideExample.Output = $"Mode: {WideTwoPaneView.Mode} at {WideTwoPaneView.Width:0} wide";

    private void OnTallModeChanged(object? sender, EventArgs e) =>
        TallExample.Output = $"Mode: {TallTwoPaneView.Mode} at {TallTwoPaneView.Height:0} tall";

    private void OnDualScreenInfoChanged(object? sender, PropertyChangedEventArgs e) =>
        MainThread.BeginInvokeOnMainThread(ShowDualScreenInfo);

    private void ShowDualScreenInfo()
    {
        var info = DualScreenInfo.Current;
        HingeSpanMode.Text = $"SpanMode: {info.SpanMode}";
        HingeLandscape.Text = $"IsLandscape: {info.IsLandscape}";
        HingeBounds.Text = info.HingeBounds == Rect.Zero
            ? "HingeBounds: none"
            : $"HingeBounds: {info.HingeBounds.X:0}, {info.HingeBounds.Y:0}, {info.HingeBounds.Width:0} × {info.HingeBounds.Height:0}";
    }

    private async void OnReadHingeAngleClicked(object? sender, EventArgs e)
    {
        try
        {
            // On Android the call waits for the hinge sensor, which a phone without a hinge doesn't have, so give up
            // after a moment. Other platforms return 0.
            var angle = await DualScreenInfo.Current.GetHingeAngleAsync().WaitAsync(HingeAngleTimeout);
            HingeAngle.Text = $"Hinge angle: {angle}°";
            HingeExample.Output = $"GetHingeAngleAsync returned {angle}";
        }
        catch (TimeoutException)
        {
            HingeAngle.Text = "Hinge angle: no hinge sensor";
            HingeExample.Output = "GetHingeAngleAsync didn't return: this device has no hinge sensor";
        }
    }
}
