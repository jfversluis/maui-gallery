using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Collections;

/// <summary>The view model behind the "Bind the position and current item" CarouselView sample.</summary>
public sealed partial class TripCarouselViewModel : ObservableObject
{
    public IReadOnlyList<Trip> Trips { get; } = TripData.All;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PositionText))]
    [NotifyCanExecuteChangedFor(nameof(PreviousCommand), nameof(NextCommand))]
    public partial int Position { get; set; }

    [ObservableProperty]
    public partial Trip? CurrentItem { get; set; }

    public string PositionText => $"{Position + 1} of {Trips.Count}";

    [RelayCommand(CanExecute = nameof(CanGoPrevious))]
    private void Previous() => Position--;

    [RelayCommand(CanExecute = nameof(CanGoNext))]
    private void Next() => Position++;

    private bool CanGoPrevious() => Position > 0;

    private bool CanGoNext() => Position < Trips.Count - 1;
}
