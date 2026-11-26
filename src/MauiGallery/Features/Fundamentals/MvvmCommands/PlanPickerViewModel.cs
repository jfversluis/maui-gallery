using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.Fundamentals;

/// <summary>The view model behind the "CanExecute and CommandParameter" sample.</summary>
public sealed partial class PlanPickerViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ChoosePlanCommand))]
    public partial string CurrentPlan { get; set; } = "Free";

    [RelayCommand(CanExecute = nameof(CanChoosePlan))]
    private void ChoosePlan(string plan) => CurrentPlan = plan;

    private bool CanChoosePlan(string plan) => plan != CurrentPlan;
}
