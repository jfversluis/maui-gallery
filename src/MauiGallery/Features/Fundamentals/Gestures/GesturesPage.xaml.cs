using System.Globalization;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class GesturesPage : SamplePage
{
    private const double MinScale = 0.5;
    private const double MaxScale = 2.5;
    private const double DotRadius = 8;

    private double _panStartX;
    private double _panStartY;

    public GesturesPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        TapExample.NativeTarget = TapTile;
        LongPressExample.NativeTarget = LongPressTile;
        PanPinchExample.NativeTarget = PanCard;
        SwipeExample.NativeTarget = SwipeTile;
        PointerExample.NativeTarget = PointerPad;

        // The page scrolls; keep it still while a drag, pinch or swipe that starts on these views is in progress.
        ScrollCapture.CaptureDrags(PanCard);
        ScrollCapture.CaptureDrags(SwipeTile);
        OnTapButtonsChanged(this, EventArgs.Empty);
    }

    private static string Describe(Point? position) =>
        position is { } p ? string.Create(CultureInfo.InvariantCulture, $"({p.X:0}, {p.Y:0})") : "(unknown)";

    private void OnTapButtonsChanged(object? sender, EventArgs e)
    {
        var buttons = TapButtons.SelectedIndex switch
        {
            1 => ButtonsMask.Secondary,
            2 => ButtonsMask.Primary | ButtonsMask.Secondary,
            _ => ButtonsMask.Primary,
        };

        TapRecognizer.Buttons = buttons;
        TapButtonsXaml.Value = buttons.ToString();
        TapButtonsCode.Value = buttons switch
        {
            ButtonsMask.Secondary => "ButtonsMask.Secondary",
            ButtonsMask.Primary => "ButtonsMask.Primary",
            _ => "ButtonsMask.Primary | ButtonsMask.Secondary",
        };
    }

    private void OnTileTapped(object? sender, TappedEventArgs e)
    {
        var taps = TapRecognizer.NumberOfTapsRequired switch
        {
            2 => "Double-tapped",
            3 => "Triple-tapped",
            _ => "Tapped",
        };
        var button = e.Buttons == ButtonsMask.Secondary ? " with the secondary button" : string.Empty;
        TapLabel.Text = taps;
        TapExample.Output = $"{taps}{button} at {Describe(e.GetPosition(TapTile))}";
    }

    private void OnLongPressing(object? sender, LongPressingEventArgs e)
    {
        if (e.Status == GestureStatus.Started)
        {
            LongPressLabel.Text = "Keep holding";
            LongPressExample.Output = "LongPressing: Started";
            LongPressTile.CancelAnimations();
            _ = LongPressTile.ScaleToAsync(0.92, 150, Easing.CubicOut);
        }
        else if (e.Status is GestureStatus.Completed or GestureStatus.Canceled)
        {
            // Completed arrives after LongPressed, which already reported the press.
            if (e.Status == GestureStatus.Canceled)
            {
                LongPressLabel.Text = "Press and hold";
                LongPressExample.Output = "LongPressing: Canceled";
            }

            _ = LongPressTile.ScaleToAsync(1, 150, Easing.CubicOut);
        }
    }

    private void OnLongPressed(object? sender, LongPressedEventArgs e)
    {
        LongPressLabel.Text = "Long pressed";

        // Not e.GetPosition: on Android in .NET MAUI 11.0 it returns pixels instead of device-independent units.
        LongPressExample.Output = "LongPressed";
    }

    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        switch (e.StatusType)
        {
            case GestureStatus.Started:
                _panStartX = PanCard.TranslationX;
                _panStartY = PanCard.TranslationY;
                break;
            case GestureStatus.Running:
                MoveCard(_panStartX + e.TotalX, _panStartY + e.TotalY);
                PanPinchExample.Output = string.Create(CultureInfo.InvariantCulture, $"Panned to ({PanCard.TranslationX:0}, {PanCard.TranslationY:0})");
                break;
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                ScrollCapture.End(PanCard);
                break;
        }
    }

    private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        switch (e.Status)
        {
            case GestureStatus.Running:
                // Scale is the change since the last update, so multiply it in.
                PanCard.Scale = Math.Clamp(PanCard.Scale * e.Scale, MinScale, MaxScale);
                MoveCard(PanCard.TranslationX, PanCard.TranslationY);
                PanPinchExample.Output = string.Create(CultureInfo.InvariantCulture, $"Pinched to {PanCard.Scale:0.0#}×");
                break;
            case GestureStatus.Completed:
            case GestureStatus.Canceled:
                ScrollCapture.End(PanCard);
                break;
        }
    }

    // Keeps the scaled card inside the dashed area.
    private void MoveCard(double x, double y)
    {
        var maxX = Math.Max(0, (PanArea.Width - (PanCard.Width * PanCard.Scale)) / 2);
        var maxY = Math.Max(0, (PanArea.Height - (PanCard.Height * PanCard.Scale)) / 2);
        PanCard.TranslationX = Math.Clamp(x, -maxX, maxX);
        PanCard.TranslationY = Math.Clamp(y, -maxY, maxY);
    }

    private async void OnPanResetClicked(object? sender, EventArgs e)
    {
        await Task.WhenAll(
            PanCard.TranslateToAsync(0, 0, 250, Easing.CubicOut),
            PanCard.ScaleToAsync(1, 250, Easing.CubicOut));
        PanPinchExample.Output = "Reset";
    }

    private async void OnSwiped(object? sender, SwipedEventArgs e)
    {
        ScrollCapture.End(SwipeTile);
        SwipeLabel.Text = $"Swiped {e.Direction.ToString().ToLowerInvariant()}";
        SwipeExample.Output = $"Swiped {e.Direction}";

        var (x, y) = e.Direction switch
        {
            SwipeDirection.Left => (-40, 0),
            SwipeDirection.Right => (40, 0),
            SwipeDirection.Up => (0, -30),
            _ => (0, 30),
        };
        SwipeTile.CancelAnimations();
        await SwipeTile.TranslateToAsync(x, y, 120, Easing.CubicOut);
        await SwipeTile.TranslateToAsync(0, 0, 300, Easing.SpringOut);
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        PointerDot.IsVisible = true;
        MoveDot(e);
        PointerExample.Output = $"Entered at {Describe(e.GetPosition(PointerPad))}";
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        PointerDot.IsVisible = true;
        MoveDot(e);
    }

    private void OnPointerPressed(object? sender, PointerEventArgs e)
    {
        // Touch presses without entering first.
        PointerDot.IsVisible = true;
        MoveDot(e);
        PointerDot.Scale = 1.5;
        PointerExample.Output = $"Pressed ({e.Button}) at {Describe(e.GetPosition(PointerPad))}";
    }

    private void OnPointerReleased(object? sender, PointerEventArgs e)
    {
        PointerDot.Scale = 1;
        PointerExample.Output = $"Released ({e.Button}) at {Describe(e.GetPosition(PointerPad))}";
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        PointerDot.IsVisible = false;
        PointerDot.Scale = 1;
        PointerLabel.Text = "Move the pointer here";
        PointerExample.Output = "Exited";
    }

    private void MoveDot(PointerEventArgs e)
    {
        if (e.GetPosition(PointerPad) is not { } position)
        {
            return;
        }

        PointerDot.TranslationX = position.X - DotRadius;
        PointerDot.TranslationY = position.Y - DotRadius;
        PointerLabel.Text = Describe(position);
    }
}
