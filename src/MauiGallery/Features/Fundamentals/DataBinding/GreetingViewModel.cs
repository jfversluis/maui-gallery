using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.Fundamentals;

/// <summary>The view model behind the "Compiled bindings with x:DataType" sample.</summary>
public sealed partial class GreetingViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Greeting))]
    public partial string? Name { get; set; } = "Maui";

    public string Greeting => string.IsNullOrWhiteSpace(Name) ? "Hello!" : $"Hello, {Name.Trim()}!";
}
