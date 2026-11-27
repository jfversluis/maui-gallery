using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.Fundamentals;

/// <summary>The view model behind the "C# expressions in XAML" sample.</summary>
public sealed partial class TicketOrderViewModel : ObservableObject
{
    [ObservableProperty]
    public partial double Quantity { get; set; } = 2;

    public decimal UnitPrice { get; } = 24.50m;
}
