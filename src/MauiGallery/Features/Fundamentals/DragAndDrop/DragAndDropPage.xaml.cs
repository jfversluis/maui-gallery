using System.ComponentModel;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class DragAndDropPage : SamplePage
{
    private const string FoodKey = "Food";

    private static readonly Dictionary<string, FoodItem> Foods = new()
    {
        ["Apple"] = new("Apple", IsFruit: true),
        ["Banana"] = new("Banana", IsFruit: true),
        ["Carrot"] = new("Carrot", IsFruit: false),
        ["Broccoli"] = new("Broccoli", IsFruit: false),
    };

    private readonly List<string> _basket = [];
    private readonly ReorderViewModel _reorder = new();

    public DragAndDropPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        TextExample.NativeTarget = TextChip;
        DataExample.NativeTarget = Basket;
        ReorderSample.BindingContext = _reorder;
        ReorderExample.NativeTarget = ReorderSample;
        _reorder.PropertyChanged += OnReorderChanged;
    }

    private Brush TargetBrush(string key) => (Brush)Resources[key];

    private void OnTextDragOver(object? sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;
        TextTarget.Stroke = TargetBrush("TargetActiveStroke");
    }

    private void OnTextDragLeave(object? sender, DragEventArgs e) => TextTarget.Stroke = TargetBrush("TargetStroke");

    private async void OnTextDrop(object? sender, DropEventArgs e)
    {
        TextTarget.Stroke = TargetBrush("TargetStroke");
        var text = await e.Data.GetTextAsync();
        TextTargetLabel.Text = string.IsNullOrEmpty(text) ? "That drop had no text" : text;
        TextExample.Output = $"Dropped \"{text}\"";
    }

    private void OnFoodDragStarting(object? sender, DragStartingEventArgs e)
    {
        if (sender is Label { Text: { } name } && Foods.TryGetValue(name, out var food))
        {
            e.Data.Properties[FoodKey] = food;
            DataExample.Output = $"Dragging {food.Name}";
        }
    }

    // A refused drop raises neither Drop nor DragLeave on the target.
    private void OnFoodDropCompleted(object? sender, DropCompletedEventArgs e) => Basket.Stroke = TargetBrush("TargetStroke");

    private void OnBasketDragOver(object? sender, DragEventArgs e)
    {
        var isFruit = e.Data.Properties.TryGetValue(FoodKey, out var value) && value is FoodItem { IsFruit: true };
        e.AcceptedOperation = isFruit ? DataPackageOperation.Copy : DataPackageOperation.None;
        Basket.Stroke = TargetBrush(isFruit ? "TargetActiveStroke" : "TargetRejectStroke");
    }

    private void OnBasketDragLeave(object? sender, DragEventArgs e) => Basket.Stroke = TargetBrush("TargetStroke");

    private void OnBasketDrop(object? sender, DropEventArgs e)
    {
        Basket.Stroke = TargetBrush("TargetStroke");
        if (e.Data.Properties.TryGetValue(FoodKey, out var value) && value is FoodItem { IsFruit: true } food)
        {
            _basket.Add(food.Name);
            BasketLabel.Text = $"Fruit basket: {string.Join(", ", _basket)}";
            DataExample.Output = $"{food.Name} is in the basket";
        }
    }

    private void OnBasketEmptyClicked(object? sender, EventArgs e)
    {
        _basket.Clear();
        BasketLabel.Text = "Fruit basket: empty";
        DataExample.Output = "Emptied the basket";
    }

    private void OnReorderChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ReorderViewModel.Order))
        {
            ReorderExample.Output = _reorder.Order;
        }
    }
}
