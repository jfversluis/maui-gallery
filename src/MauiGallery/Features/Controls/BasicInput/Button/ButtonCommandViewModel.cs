using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.BasicInput;

/// <summary>The view model behind the "Bind to a command" Button sample.</summary>
public sealed partial class ButtonCommandViewModel : ObservableObject
{
    private const int MaxItems = 3;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CartSummary))]
    [NotifyCanExecuteChangedFor(nameof(AddToCartCommand))]
    public partial int ItemCount { get; set; }

    public string CartSummary => ItemCount switch
    {
        0 => "Your cart is empty",
        MaxItems => $"{MaxItems} items: that's the limit",
        1 => "1 item in your cart",
        _ => $"{ItemCount} items in your cart",
    };

    [RelayCommand(CanExecute = nameof(CanAddToCart))]
    private void AddToCart() => ItemCount++;

    private bool CanAddToCart() => ItemCount < MaxItems;
}
