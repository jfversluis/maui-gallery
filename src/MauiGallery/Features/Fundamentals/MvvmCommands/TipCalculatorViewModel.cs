using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.Fundamentals;

/// <summary>The view model behind the "Observable properties" sample: inputs, and values computed from them.</summary>
public sealed partial class TipCalculatorViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string? BillText { get; set; } = "84.00";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Tip))]
    [NotifyPropertyChangedFor(nameof(Total))]
    [NotifyPropertyChangedFor(nameof(PerPerson))]
    public partial decimal Bill { get; set; } = 84m;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Tip))]
    [NotifyPropertyChangedFor(nameof(Total))]
    [NotifyPropertyChangedFor(nameof(PerPerson))]
    public partial double TipPercent { get; set; } = 15;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PerPerson))]
    public partial double People { get; set; } = 2;

    public decimal Tip => Math.Round(Bill * (decimal)Math.Round(TipPercent) / 100m, 2);

    public decimal Total => Bill + Tip;

    public decimal PerPerson => Math.Round(Total / (decimal)Math.Max(1, People), 2);

    // The generator calls this partial method whenever BillText changes.
    partial void OnBillTextChanged(string? value) =>
        Bill = decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out var bill) && bill >= 0 ? bill : 0;
}
