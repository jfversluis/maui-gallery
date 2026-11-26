using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.Controls.Pickers;

/// <summary>The view model behind the "Limit the range" DatePicker sample.</summary>
public sealed partial class DatePickerRangeViewModel : ObservableObject
{
    public DateTime Today { get; } = DateTime.Today;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LastBookableDate))]
    public partial int DaysAhead { get; set; } = 30;

    public DateTime LastBookableDate => Today.AddDays(DaysAhead);

    [ObservableProperty]
    public partial DateTime? CheckIn { get; set; } = DateTime.Today.AddDays(1);
}
