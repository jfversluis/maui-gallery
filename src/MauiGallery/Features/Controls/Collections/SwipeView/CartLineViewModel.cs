using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Collections;

/// <summary>The view model behind the "Put any view in a SwipeItemView" SwipeView sample.</summary>
public sealed partial class CartLineViewModel : ObservableObject
{
    private const decimal UnitPrice = 4.50m;

    public string Name { get; } = "Oat milk latte";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Summary))]
    [NotifyCanExecuteChangedFor(nameof(DecreaseCommand))]
    public partial int Quantity { get; set; } = 1;

    public string Summary => $"Qty {Quantity} · {(Quantity * UnitPrice).ToString("C", CultureInfo.GetCultureInfo("en-US"))}";

    [RelayCommand]
    private void Increase() => Quantity++;

    [RelayCommand(CanExecute = nameof(CanDecrease))]
    private void Decrease() => Quantity--;

    private bool CanDecrease() => Quantity > 1;
}
