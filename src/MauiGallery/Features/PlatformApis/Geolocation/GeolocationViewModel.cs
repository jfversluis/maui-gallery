using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the Geolocation samples: the last known location, a fresh fix, and live updates.</summary>
public sealed partial class GeolocationViewModel(IGeolocation geolocation) : ObservableObject
{
    private const string InUseElsewhere = "Another window is following the location. Stop it there first.";

    private Location? _previous;

    // Stop() bumps it, so a start that was still waiting for permission knows the page has gone.
    private int _generation;

    // Last known location
    [ObservableProperty]
    public partial string LastKnownCoordinates { get; set; } = "Not read yet";

    [ObservableProperty]
    public partial string? LastKnownDetails { get; set; }

    // Current location
    /// <summary>Index into Lowest, Low, Medium, High, Best.</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Accuracy))]
    public partial int AccuracyIndex { get; set; } = 2;

    public GeolocationAccuracy Accuracy => (GeolocationAccuracy)(AccuracyIndex + 1);

    [ObservableProperty]
    public partial double TimeoutSeconds { get; set; } = 10;

    [ObservableProperty]
    public partial string CurrentCoordinates { get; set; } = "Not located yet";

    [ObservableProperty]
    public partial string? CurrentDetails { get; set; }

    // Compiled bindings can't see the generated GetCurrentCancelCommand, so expose it through a declared property.
    public ICommand CancelCurrentCommand => GetCurrentCancelCommand;

    // Listening
    [ObservableProperty]
    public partial int ListenAccuracyIndex { get; set; } = 3;

    [ObservableProperty]
    public partial double MinimumSeconds { get; set; } = 1;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotListening), nameof(ListenButtonText))]
    public partial bool IsListening { get; set; }

    public bool IsNotListening => !IsListening;

    public string ListenButtonText => IsListening ? "Stop listening" : "Start listening";

    [ObservableProperty]
    public partial string ListenCoordinates { get; set; } = "Not listening";

    [ObservableProperty]
    public partial string? ListenDetails { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Progress))]
    public partial int UpdateCount { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Progress))]
    public partial double MetersTravelled { get; set; }

    public string Progress => $"{UpdateCount} {(UpdateCount == 1 ? "update" : "updates")} · {MetersTravelled:N0} m travelled";

    [ObservableProperty]
    public partial string? Error { get; set; }

    /// <summary>Set when Android only allows the approximate location.</summary>
    [ObservableProperty]
    public partial string? Note { get; set; }

    /// <summary>Whether location services are on for the whole device.</summary>
    public bool IsEnabled => geolocation.IsEnabled;

    [RelayCommand]
    private async Task GetLastKnownAsync()
    {
        await RunAsync(async () =>
        {
            // Fast and battery-friendly, but it can be old, or null if the device hasn't located itself lately.
            var location = await geolocation.GetLastKnownLocationAsync();
            LastKnownCoordinates = location is null ? "No location cached yet" : LocationText.Coordinates(location);
            LastKnownDetails = location is null ? "Get the current location first" : LocationText.Details(location);
        });
    }

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task GetCurrentAsync(CancellationToken cancellationToken)
    {
        CurrentCoordinates = "Locating…";
        CurrentDetails = null;
        await RunAsync(async () =>
        {
            try
            {
                var request = new GeolocationRequest(Accuracy, TimeSpan.FromSeconds(TimeoutSeconds));
                var location = await geolocation.GetLocationAsync(request, cancellationToken);
                CurrentCoordinates = location is null ? "No location within the timeout" : LocationText.Coordinates(location);
                CurrentDetails = location is null ? "Try a lower accuracy or a longer timeout" : LocationText.Details(location);
            }
            catch (OperationCanceledException)
            {
                CurrentCoordinates = "Cancelled";
            }
        });
    }

    [RelayCommand]
    private async Task ToggleListeningAsync()
    {
        if (IsListening)
        {
            StopListening();
            return;
        }

        // There's one listener for the whole app: another window may be using it.
        if (geolocation.IsListeningForeground)
        {
            Error = InUseElsewhere;
            return;
        }

        var generation = _generation;
        await RunAsync(async () =>
        {
            geolocation.LocationChanged += OnLocationChanged;
            geolocation.ListeningFailed += OnListeningFailed;
            var started = false;
            try
            {
                var request = new GeolocationListeningRequest((GeolocationAccuracy)(ListenAccuracyIndex + 1), TimeSpan.FromSeconds(MinimumSeconds));
                started = await geolocation.StartListeningForegroundAsync(request);
                if (started && generation != _generation)
                {
                    // The page went away while it was starting, for example while the permission prompt was up:
                    // don't leave the GPS on.
                    geolocation.StopListeningForeground();
                    started = false;
                }
            }
            catch (InvalidOperationException)
            {
                Error = InUseElsewhere;
            }
            finally
            {
                if (!started)
                {
                    Unsubscribe();
                }
            }

            if (started)
            {
                IsListening = true;
                _previous = null;
                UpdateCount = 0;
                MetersTravelled = 0;
                ListenCoordinates = "Waiting for the first update…";
                ListenDetails = null;
            }
        });
    }

    /// <summary>Stops every request. Call it when the page disappears: a listening app keeps the GPS on.</summary>
    public void Stop()
    {
        _generation++;
        if (GetCurrentCommand.IsRunning)
        {
            GetCurrentCommand.Cancel();
        }

        StopListening();
    }

    private void StopListening()
    {
        if (IsListening)
        {
            geolocation.StopListeningForeground();
            IsListening = false;
            ListenCoordinates = UpdateCount == 0 ? "Not listening" : ListenCoordinates;
        }

        Unsubscribe();
    }

    private void Unsubscribe()
    {
        geolocation.LocationChanged -= OnLocationChanged;
        geolocation.ListeningFailed -= OnListeningFailed;
    }

    private void OnLocationChanged(object? sender, GeolocationLocationChangedEventArgs e) => MainThread.BeginInvokeOnMainThread(() =>
    {
        var location = e.Location;
        if (_previous is not null)
        {
            MetersTravelled += Location.CalculateDistance(_previous, location, DistanceUnits.Kilometers) * 1000;
        }

        _previous = location;
        UpdateCount++;
        ListenCoordinates = LocationText.Coordinates(location);
        ListenDetails = LocationText.Details(location);
    });

    private void OnListeningFailed(object? sender, GeolocationListeningFailedEventArgs e) => MainThread.BeginInvokeOnMainThread(() =>
    {
        IsListening = false;
        Unsubscribe();
        ListenCoordinates = e.Error == GeolocationError.Unauthorized ? "Location access was turned off" : "The location is unavailable";
    });

    private async Task RunAsync(Func<Task> action)
    {
        Error = null;
        try
        {
            await action();
        }
        catch (FeatureNotEnabledException)
        {
            Error = "Location services are turned off for this device.";
        }
        catch (FeatureNotSupportedException)
        {
            Error = "This device can't tell where it is.";
        }
        catch (PermissionException)
        {
            // The location APIs ask for permission themselves, and throw this when the user says no.
            Error = "Location access is off. Turn it on in Settings to try these samples.";
            SemanticScreenReader.Announce(Error);
        }

        // On Android, Restricted means the user chose the approximate location. The location APIs accept it, and
        // work to within a few kilometers. (On iOS Restricted means a policy blocks location; reduced accuracy is
        // still Granted.)
        var isApproximate = OperatingSystem.IsAndroid()
            && await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>() == PermissionStatus.Restricted;
        Note = isApproximate ? "You allowed the approximate location only, so expect city-level accuracy." : null;
    }
}
