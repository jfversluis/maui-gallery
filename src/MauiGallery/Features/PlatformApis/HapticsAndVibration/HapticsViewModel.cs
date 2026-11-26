using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the haptic feedback and vibration samples.</summary>
public sealed partial class HapticsViewModel(IHapticFeedback haptics, IVibration vibration) : ObservableObject
{
    private int _lastStep = 5;

    public bool IsHapticsSupported { get; } = haptics.IsSupported;

    public bool IsVibrationSupported { get; } = vibration.IsSupported;

    [ObservableProperty]
    public partial string? LastPerformed { get; set; }

    // Haptics on a custom control
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Step))]
    public partial double Level { get; set; } = 5;

    public int Step => (int)Math.Round(Level);

    [ObservableProperty]
    public partial bool UseHaptics { get; set; } = true;

    // Vibration
    [ObservableProperty]
    public partial double DurationMilliseconds { get; set; } = 500;

    [ObservableProperty]
    public partial string? VibrationStatus { get; set; }

    /// <summary>Performs a haptic; the parameter is a <see cref="HapticFeedbackType"/> name.</summary>
    [RelayCommand(CanExecute = nameof(IsHapticsSupported))]
    private void Perform(string type)
    {
        var feedback = Enum.Parse<HapticFeedbackType>(type);
        haptics.Perform(feedback);
        LastPerformed = $"Performed {feedback}";
    }

    [RelayCommand(CanExecute = nameof(IsVibrationSupported))]
    private void Vibrate()
    {
        var duration = TimeSpan.FromMilliseconds(Math.Round(DurationMilliseconds));
        vibration.Vibrate(duration);
        VibrationStatus = $"Vibrating for {duration.TotalMilliseconds:0} ms";
    }

    [RelayCommand(CanExecute = nameof(IsVibrationSupported))]
    private void Cancel()
    {
        vibration.Cancel();
        VibrationStatus = "Cancelled";
    }

    /// <summary>Stops a vibration that's still going. Call it when the page disappears.</summary>
    public void Stop()
    {
        if (IsVibrationSupported)
        {
            vibration.Cancel();
        }
    }

    // One click per whole step, like the detents of a physical dial.
    partial void OnLevelChanged(double value)
    {
        var step = (int)Math.Round(value);
        if (step != _lastStep)
        {
            _lastStep = step;
            if (UseHaptics && IsHapticsSupported)
            {
                haptics.Perform(HapticFeedbackType.Click);
            }
        }
    }
}
