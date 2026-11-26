using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.StatusAndInfo;

/// <summary>The view model behind the "Report progress from async work" ProgressBar sample.</summary>
public sealed partial class ProgressBarDownloadViewModel : ObservableObject
{
    [ObservableProperty]
    public partial double Progress { get; set; }

    [ObservableProperty]
    public partial string Status { get; set; } = "Ready to download";

    // MAUI's compiled bindings can't see the generated DownloadCancelCommand, so expose it through a declared property.
    public ICommand CancelCommand => DownloadCancelCommand;

    [RelayCommand(IncludeCancelCommand = true)]
    private async Task DownloadAsync(CancellationToken cancellationToken)
    {
        var progress = new Progress<double>(value => Progress = value);
        Status = "Downloading…";

        try
        {
            await SimulateDownloadAsync(progress, cancellationToken);
            Status = "Download complete";
        }
        catch (OperationCanceledException)
        {
            Progress = 0;
            Status = "Download cancelled";
        }
    }

    // Stands in for a real download that reports how much of the file has arrived.
    private static async Task SimulateDownloadAsync(IProgress<double> progress, CancellationToken cancellationToken)
    {
        const int Chunks = 25;
        for (var chunk = 1; chunk <= Chunks; chunk++)
        {
            await Task.Delay(120, cancellationToken);
            progress.Report((double)chunk / Chunks);
        }
    }
}
