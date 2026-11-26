using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.BasicInput;

/// <summary>The view model behind the "One command, several buttons" ImageButton sample.</summary>
public sealed partial class ImageButtonReactionsViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Summary { get; set; } = "How do you feel about this?";

    [RelayCommand]
    private void React(string reaction) => Summary = $"You reacted with {reaction}";
}
