using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the Flashlight samples: a switch, and an SOS signal in Morse code.</summary>
public sealed partial class FlashlightViewModel(IFlashlight flashlight) : ObservableObject
{
    // S O S in Morse code: light on for a dot (1 unit) or a dash (3), off for 1 unit between them and 3 between letters.
    private static readonly (bool On, int Units)[] Sos =
    [
        (true, 1), (false, 1), (true, 1), (false, 1), (true, 1), (false, 3),
        (true, 3), (false, 1), (true, 3), (false, 1), (true, 3), (false, 3),
        (true, 1), (false, 1), (true, 1), (false, 1), (true, 1), (false, 7),
    ];

    // TurnOffAsync bumps it, so a TurnOnAsync still waiting for camera permission knows not to leave the light on.
    private int _generation;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ToggleCommand), nameof(SignalCommand))]
    public partial bool IsSupported { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ToggleText))]
    public partial bool IsOn { get; set; }

    public string ToggleText => IsOn ? "Turn off" : "Turn on";

    [ObservableProperty]
    public partial string Message { get; set; } = "Checking for a flashlight…";

    /// <summary>The length of a Morse dot, in milliseconds.</summary>
    [ObservableProperty]
    public partial double UnitMilliseconds { get; set; } = 200;

    [ObservableProperty]
    public partial string? SignalStatus { get; set; }

    // Compiled bindings can't see the generated SignalCancelCommand, so expose it through a declared property.
    public ICommand CancelSignalCommand => SignalCancelCommand;

    public async Task CheckSupportAsync()
    {
        IsSupported = await flashlight.IsSupportedAsync();
        Message = IsSupported ? "The flashlight is off" : "This device has no flashlight.";
    }

    [RelayCommand(CanExecute = nameof(IsSupported))]
    private async Task ToggleAsync()
    {
        if (SignalCommand.IsRunning)
        {
            SignalCommand.Cancel();
        }

        await SetAsync(!IsOn);
        Message = IsOn ? "The flashlight is on" : "The flashlight is off";
    }

    [RelayCommand(CanExecute = nameof(IsSupported), IncludeCancelCommand = true)]
    private async Task SignalAsync(CancellationToken cancellationToken)
    {
        SignalStatus = "Signaling SOS…";
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                foreach (var (on, units) in Sos)
                {
                    await SetAsync(on);
                    await Task.Delay(TimeSpan.FromMilliseconds(units * UnitMilliseconds), cancellationToken);
                }
            }
        }
        catch (OperationCanceledException)
        {
            SignalStatus = "Stopped";
        }
        finally
        {
            await SetAsync(false);
        }
    }

    /// <summary>Stops the signal and turns the light off. Call it when the page disappears.</summary>
    public async Task TurnOffAsync()
    {
        _generation++;
        if (SignalCommand.IsRunning)
        {
            SignalCommand.Cancel();
        }

        await SetAsync(false);
    }

    private async Task SetAsync(bool on)
    {
        // Skip calls that change nothing: on Android even TurnOffAsync asks for camera permission.
        if (!IsSupported || on == IsOn)
        {
            return;
        }

        var generation = _generation;
        try
        {
            if (on)
            {
                await flashlight.TurnOnAsync();
                if (generation != _generation)
                {
                    // The page went away while this waited for permission.
                    await flashlight.TurnOffAsync();
                    return;
                }
            }
            else
            {
                await flashlight.TurnOffAsync();
            }

            IsOn = on;
        }
        catch (FeatureNotSupportedException)
        {
            IsSupported = false;
            Message = "This device has no flashlight.";
        }
        catch (PermissionException)
        {
            // On Android, the flashlight belongs to the camera, so it needs camera permission.
            Message = "The flashlight needs camera access. Turn it on in Settings.";
        }
    }
}
