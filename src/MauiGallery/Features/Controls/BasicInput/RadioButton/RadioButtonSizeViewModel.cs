using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Controls.BasicInput;

/// <summary>The view model behind the "Bind the selection with RadioButtonGroup" RadioButton sample.</summary>
public sealed partial class RadioButtonSizeViewModel : ObservableObject
{
    [ObservableProperty]
    public partial string Size { get; set; } = "M";

    [RelayCommand]
    private void Reset() => Size = "M";
}
