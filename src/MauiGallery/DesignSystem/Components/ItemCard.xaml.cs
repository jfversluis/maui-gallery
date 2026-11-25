using System.Windows.Input;

namespace MauiGallery.DesignSystem.Components;

/// <summary>A tappable tile for a gallery item. Its binding context is the <c>GalleryItem</c> it shows.</summary>
public partial class ItemCard : ContentView
{
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ItemCard));

    public ItemCard() => InitializeComponent();

    /// <summary>Invoked with the card's <c>GalleryItem</c> as parameter.</summary>
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }
}
