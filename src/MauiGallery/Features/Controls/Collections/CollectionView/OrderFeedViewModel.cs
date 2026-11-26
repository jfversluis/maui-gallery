using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.Collections;

/// <summary>An order in the incremental loading sample.</summary>
public sealed record Order(int Number, int ItemCount, decimal Total, DateTime PlacedOn)
{
    public string Title => $"Order #{Number}";

    public string Details => $"{(ItemCount == 1 ? "1 item" : $"{ItemCount} items")} · {PlacedOn:MMM d}";

    public string TotalText => Total.ToString("C", CultureInfo.GetCultureInfo("en-US"));
}

/// <summary>The view model behind the "Load more as the user scrolls" CollectionView sample.</summary>
public sealed partial class OrderFeedViewModel : ObservableObject
{
    public const int PageSize = 20;
    public const int TotalOrders = 100;

    private static readonly DateTime Today = new(2026, 11, 26);

    public OrderFeedViewModel() => AddPage();

    public ObservableCollection<Order> Orders { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Status))]
    [NotifyCanExecuteChangedFor(nameof(ResetCommand))]
    public partial bool IsLoading { get; private set; }

    public bool HasMore => Orders.Count < TotalOrders;

    public string Status => IsLoading ? "Loading more orders…" : HasMore ? $"{Orders.Count} of {TotalOrders} orders" : $"All {TotalOrders} orders loaded";

    [RelayCommand]
    private async Task LoadMoreAsync()
    {
        if (IsLoading || !HasMore)
        {
            return;
        }

        IsLoading = true;

        // Stands in for a call to a web service.
        await Task.Delay(800);
        AddPage();

        IsLoading = false;
        OnPropertyChanged(nameof(HasMore));
    }

    [RelayCommand(CanExecute = nameof(CanReset))]
    private void Reset()
    {
        Orders.Clear();
        AddPage();
        OnPropertyChanged(nameof(HasMore));
        OnPropertyChanged(nameof(Status));
    }

    private bool CanReset() => !IsLoading;

    private void AddPage()
    {
        var start = Orders.Count;
        for (var i = start; i < Math.Min(start + PageSize, TotalOrders); i++)
        {
            Orders.Add(new Order(1001 + i, 1 + (i * 7 % 5), 12.5m + (i * 37 % 140), Today.AddDays(-i / 3)));
        }
    }
}
