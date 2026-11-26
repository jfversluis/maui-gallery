using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Media;

/// <summary>
/// State shared by the BlazorWebView sample's Razor component and its .NET MAUI controls. It is a DI singleton, so
/// both sides get the same instance: a change on one side shows on the other.
/// </summary>
public sealed partial class PackingListViewModel : ObservableObject
{
    public PackingListViewModel()
    {
        Items.CollectionChanged += OnItemsChanged;
        foreach (var title in new[] { "Passport", "Charger", "Sunglasses" })
        {
            Items.Add(new PackingItem(title));
        }

        Items[0].IsPacked = true;
    }

    /// <summary>Raised after any change, for UI that doesn't data-bind, such as a Razor component.</summary>
    public event EventHandler? Changed;

    public ObservableCollection<PackingItem> Items { get; } = [];

    public string Summary => $"{Items.Count(i => i.IsPacked)} of {Items.Count} packed";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCommand))]
    public partial string NewItem { get; set; } = string.Empty;

    private bool CanAdd => !string.IsNullOrWhiteSpace(NewItem);

    [RelayCommand(CanExecute = nameof(CanAdd))]
    private void Add()
    {
        Items.Add(new PackingItem(NewItem.Trim()));
        NewItem = string.Empty;
    }

    private void OnItemsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var item in e.NewItems?.OfType<PackingItem>() ?? [])
        {
            item.PropertyChanged += OnItemChanged;
        }

        foreach (var item in e.OldItems?.OfType<PackingItem>() ?? [])
        {
            item.PropertyChanged -= OnItemChanged;
        }

        NotifyChanged();
    }

    private void OnItemChanged(object? sender, PropertyChangedEventArgs e) => NotifyChanged();

    private void NotifyChanged()
    {
        OnPropertyChanged(nameof(Summary));
        Changed?.Invoke(this, EventArgs.Empty);
    }
}

public sealed partial class PackingItem(string title) : ObservableObject
{
    public string Title { get; } = title;

    [ObservableProperty]
    public partial bool IsPacked { get; set; }
}
