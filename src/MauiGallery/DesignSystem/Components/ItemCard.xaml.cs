using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using MauiGallery.Core.Catalog;
using MauiGallery.Services;

namespace MauiGallery.DesignSystem.Components;

/// <summary>
/// A tappable tile for a gallery item. Its binding context is the <see cref="GalleryItem"/> it shows. On desktop it
/// also has a context menu (right-click) to open the item in a new window, favorite it or copy its link.
/// </summary>
public partial class ItemCard : ContentView, IRecipient<FavoritesChangedMessage>
{
    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ItemCard));

    private MenuFlyoutItem? _favoriteItem;
    private ItemActions? _actions;

    public ItemCard() => InitializeComponent();

    /// <summary>Invoked with the card's <see cref="GalleryItem"/> as parameter.</summary>
    public ICommand? Command
    {
        get => (ICommand?)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public void Receive(FavoritesChangedMessage message) => UpdateFavoriteText();

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        // Views cannot use constructor injection, so a reusable control takes services from its MAUI context.
        var services = Handler?.MauiContext?.Services;
        if (_actions is not null || services is null)
        {
            return;
        }

        _actions = services.GetRequiredService<ItemActions>();
        _favoriteItem = new MenuFlyoutItem { Command = _actions.ToggleFavoriteCommand };
        var menu = new MenuFlyout
        {
            new MenuFlyoutItem { Text = "Open", Command = _actions.OpenCommand },
        };
        if (_actions.CanOpenWindows)
        {
            menu.Add(new MenuFlyoutItem { Text = "Open in New Window", Command = _actions.OpenInNewWindowCommand });
        }

        menu.Add(new MenuFlyoutSeparator());
        menu.Add(_favoriteItem);
        menu.Add(new MenuFlyoutItem { Text = "Copy Link", Command = _actions.CopyLinkCommand });
        foreach (var item in menu.OfType<MenuFlyoutItem>())
        {
            item.SetBinding(MenuItem.CommandParameterProperty, static (GalleryItem i) => i);
        }

        FlyoutBase.SetContextFlyout(CardBorder, menu);
        services.GetRequiredService<IMessenger>().Register(this);
        UpdateFavoriteText();
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        UpdateFavoriteText();
    }

    private void UpdateFavoriteText()
    {
        if (_favoriteItem is not null && _actions is not null && BindingContext is GalleryItem item)
        {
            _favoriteItem.Text = _actions.IsFavorite(item) ? "Remove from Favorites" : "Add to Favorites";
        }
    }
}
