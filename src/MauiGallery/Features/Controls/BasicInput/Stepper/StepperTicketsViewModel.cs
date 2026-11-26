using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.Controls.BasicInput;

/// <summary>The view model behind the "Bind a quantity" Stepper sample.</summary>
public sealed partial class StepperTicketsViewModel : ObservableObject
{
    private const decimal TicketPrice = 45m;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Total))]
    public partial double Tickets { get; set; } = 2;

    public decimal Total => (decimal)Tickets * TicketPrice;
}
