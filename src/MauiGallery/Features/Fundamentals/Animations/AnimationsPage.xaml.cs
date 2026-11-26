using System.Diagnostics;
using System.Globalization;
using MauiGallery.Features.Samples;
using IAnimationManager = Microsoft.Maui.Animations.IAnimationManager;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class AnimationsPage : SamplePage
{
    private const string HeartbeatName = "Heartbeat";

    private static readonly Dictionary<string, Easing> Easings = new()
    {
        [nameof(Easing.Linear)] = Easing.Linear,
        [nameof(Easing.SinIn)] = Easing.SinIn,
        [nameof(Easing.SinOut)] = Easing.SinOut,
        [nameof(Easing.SinInOut)] = Easing.SinInOut,
        [nameof(Easing.CubicIn)] = Easing.CubicIn,
        [nameof(Easing.CubicOut)] = Easing.CubicOut,
        [nameof(Easing.CubicInOut)] = Easing.CubicInOut,
        [nameof(Easing.BounceIn)] = Easing.BounceIn,
        [nameof(Easing.BounceOut)] = Easing.BounceOut,
        [nameof(Easing.SpringIn)] = Easing.SpringIn,
        [nameof(Easing.SpringOut)] = Easing.SpringOut,
    };

    private Action? _stopBounce;

    public AnimationsPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        BasicExample.NativeTarget = BasicCard;
        CompositeExample.NativeTarget = CompositeCard;
        CustomExample.NativeTarget = Heart;
        CancelExample.NativeTarget = Ball;
        OnBasicOptionChanged(this, EventArgs.Empty);
        Loaded += OnLoaded;
    }

    private string BasicMethod => BasicAnimation.SelectedItem as string ?? "FadeToAsync";

    private string BasicEasingName => BasicEasing.SelectedItem as string ?? nameof(Easing.Linear);

    private uint BasicLengthValue => (uint)Math.Round(BasicLength.Value);

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        // A repeating animation keeps running after the page is gone unless it's stopped.
        Heart.AbortAnimation(HeartbeatName);
        _stopBounce?.Invoke();
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        // The ticker reports whether the system allows animations (reduce motion, remove animations, animation effects).
        if (Handler?.MauiContext?.Services.GetService(typeof(IAnimationManager)) is IAnimationManager { Ticker.SystemEnabled: false })
        {
            BasicExample.Output = "Reduce motion is on: animations jump to their end values";
        }
    }

    private void OnBasicOptionChanged(object? sender, EventArgs e)
    {
        var (forward, back) = BasicTargets(BasicMethod);
        var arguments = $"{BasicLengthValue}, Easing.{BasicEasingName}";
        BasicForward.Value = $"{BasicMethod}({forward}, {arguments})";
        BasicBack.Value = $"{BasicMethod}({back}, {arguments})";
    }

    // The target values, as source text, of the animation there and back.
    private static (string Forward, string Back) BasicTargets(string method) => method switch
    {
        "FadeToAsync" => ("0.2", "1"),
        "ScaleToAsync" => ("1.4", "1"),
        "RotateToAsync" => ("180", "0"),
        "RotateYToAsync" => ("180", "0"),
        _ => ("80, 0", "0, 0"),
    };

    private async void OnBasicPlayClicked(object? sender, EventArgs e)
    {
        BasicCard.CancelAnimations();
        BasicCard.Opacity = 1;
        BasicCard.Scale = 1;
        BasicCard.Rotation = 0;
        BasicCard.RotationY = 0;
        BasicCard.TranslationX = 0;

        var method = BasicMethod;
        var easing = Easings[BasicEasingName];
        var length = BasicLengthValue;
        var stopwatch = Stopwatch.StartNew();

        // Each call returns true when the animation was cancelled, so || skips the way back.
        var cancelled = method switch
        {
            "FadeToAsync" =>
                await BasicCard.FadeToAsync(0.2, length, easing) || await BasicCard.FadeToAsync(1, length, easing),
            "ScaleToAsync" =>
                await BasicCard.ScaleToAsync(1.4, length, easing) || await BasicCard.ScaleToAsync(1, length, easing),
            "RotateToAsync" =>
                await BasicCard.RotateToAsync(180, length, easing) || await BasicCard.RotateToAsync(0, length, easing),
            "RotateYToAsync" =>
                await BasicCard.RotateYToAsync(180, length, easing) || await BasicCard.RotateYToAsync(0, length, easing),
            _ =>
                await BasicCard.TranslateToAsync(80, 0, length, easing) || await BasicCard.TranslateToAsync(0, 0, length, easing),
        };

        if (!cancelled)
        {
            BasicExample.Output = string.Create(CultureInfo.CurrentCulture, $"{method} there and back took {stopwatch.ElapsedMilliseconds:N0} ms");
        }
    }

    private async void OnTogetherClicked(object? sender, EventArgs e)
    {
        await ResetCompositeAsync();
        var stopwatch = Stopwatch.StartNew();
        await Task.WhenAll(
            CompositeCard.RotateToAsync(360, 600, Easing.CubicInOut),
            CompositeCard.ScaleToAsync(0.6, 600, Easing.CubicInOut),
            CompositeCard.FadeToAsync(0.3, 600));
        CompositeExample.Output = string.Create(CultureInfo.CurrentCulture, $"Together: {stopwatch.ElapsedMilliseconds:N0} ms");
        await ResetCompositeAsync();
    }

    private async void OnSequenceClicked(object? sender, EventArgs e)
    {
        await ResetCompositeAsync();
        var stopwatch = Stopwatch.StartNew();
        await CompositeCard.RotateToAsync(360, 600, Easing.CubicInOut);
        await CompositeCard.ScaleToAsync(0.6, 600, Easing.CubicInOut);
        await CompositeCard.FadeToAsync(0.3, 600);
        CompositeExample.Output = string.Create(CultureInfo.CurrentCulture, $"In sequence: {stopwatch.ElapsedMilliseconds:N0} ms");
        await ResetCompositeAsync();
    }

    private async Task ResetCompositeAsync()
    {
        CompositeCard.CancelAnimations();
        CompositeCard.Rotation = 0;
        await Task.WhenAll(CompositeCard.ScaleToAsync(1, 250), CompositeCard.FadeToAsync(1, 250));
    }

    private void OnBeatClicked(object? sender, EventArgs e)
    {
        var heartbeat = new Animation
        {
            { 0, 0.15, new Animation(v => Heart.Scale = v, 1, 1.3, Easing.CubicOut) },
            { 0.15, 0.3, new Animation(v => Heart.Scale = v, 1.3, 1, Easing.CubicIn) },
            { 0.3, 0.45, new Animation(v => Heart.Scale = v, 1, 1.2, Easing.CubicOut) },
            { 0.45, 0.7, new Animation(v => Heart.Scale = v, 1.2, 1, Easing.CubicIn) },
        };

        var beats = 0;
        heartbeat.Commit(Heart, HeartbeatName,
            length: (uint)Math.Round(CustomLength.Value),
            repeat: () => CustomRepeat.IsToggled,
            finished: (_, cancelled) =>
            {
                // Runs after every repetition; cancelled is true when AbortAnimation stopped it.
                Heart.Scale = 1;
                CustomExample.Output = cancelled
                    ? "Stopped with AbortAnimation"
                    : ++beats == 1 ? "1 beat" : $"{beats} beats";
            });
    }

    private void OnBeatStopClicked(object? sender, EventArgs e) => Heart.AbortAnimation(HeartbeatName);

    private async void OnBounceClicked(object? sender, EventArgs e)
    {
        if (_stopBounce is not null)
        {
            return;
        }

        using var bounce = new CancellationTokenSource();
        _stopBounce = bounce.Cancel;
        CancelExample.Output = "Bouncing";

        var stopped = false;
        while (!stopped)
        {
            stopped = await Ball.TranslateToAsync(0, -80, 400, Easing.CubicOut, bounce.Token)
                || await Ball.TranslateToAsync(0, 0, 400, Easing.CubicIn, bounce.Token);
        }

        _stopBounce = null;
        CancelExample.Output = bounce.IsCancellationRequested ? "Stopped by the cancellation token" : "Stopped by CancelAnimations";
        await Ball.TranslateToAsync(0, 0, 250, Easing.BounceOut);
    }

    private void OnCancelTokenClicked(object? sender, EventArgs e) => _stopBounce?.Invoke();

    private void OnCancelAnimationsClicked(object? sender, EventArgs e) => Ball.CancelAnimations();
}
