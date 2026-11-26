using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the "Display information" sample.</summary>
public sealed partial class DisplayInfoViewModel(IDeviceDisplay deviceDisplay) : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SizeInPixels), nameof(SizeInUnits))]
    public partial DisplayInfo Info { get; set; } = deviceDisplay.MainDisplayInfo;

    [ObservableProperty]
    public partial bool KeepScreenOn { get; set; }

    public string SizeInPixels => $"{Info.Width:0} × {Info.Height:0} px";

    // Divide by the density for the device-independent units that .NET MAUI layouts use.
    public string SizeInUnits => Info.Density > 0 ? $"{Info.Width / Info.Density:0} × {Info.Height / Info.Density:0}" : "Unknown";

    /// <summary>Starts listening for display changes. Call it when the page appears.</summary>
    public void Start()
    {
        Info = deviceDisplay.MainDisplayInfo;
        deviceDisplay.MainDisplayInfoChanged += OnMainDisplayInfoChanged;
    }

    /// <summary>Stops listening and lets the screen sleep again. Call it when the page disappears.</summary>
    public void Stop()
    {
        deviceDisplay.MainDisplayInfoChanged -= OnMainDisplayInfoChanged;
        KeepScreenOn = false;
    }

    partial void OnKeepScreenOnChanged(bool value) => deviceDisplay.KeepScreenOn = value;

    private void OnMainDisplayInfoChanged(object? sender, DisplayInfoChangedEventArgs e) => Info = e.DisplayInfo;
}
