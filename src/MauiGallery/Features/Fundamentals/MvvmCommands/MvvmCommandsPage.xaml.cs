using System.ComponentModel;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class MvvmCommandsPage : SamplePage
{
    private readonly List<string> _changedProperties = [];
    private readonly CitySearchViewModel _search = new();

    public MvvmCommandsPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        var tips = new TipCalculatorViewModel();
        tips.PropertyChanged += OnTipPropertyChanged;
        TipSample.BindingContext = tips;

        var plans = new PlanPickerViewModel();
        plans.PropertyChanged += (_, _) => PlanExample.Output = $"ChoosePlanCommand executed with \"{plans.CurrentPlan}\"";
        PlanSample.BindingContext = plans;
        PlanExample.NativeTarget = PlanFreeButton;

        _search.SearchCommand.PropertyChanged += OnSearchCommandPropertyChanged;
        SearchSample.BindingContext = _search;
        SearchExample.NativeTarget = SearchButton;
    }

    // Collects the notifications one change raises, then shows them together.
    private void OnTipPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_changedProperties.Count == 0)
        {
            Dispatcher.Dispatch(() =>
            {
                TipExample.Output = $"PropertyChanged: {string.Join(", ", _changedProperties)}";
                _changedProperties.Clear();
            });
        }

        _changedProperties.Add(e.PropertyName ?? string.Empty);
    }

    private void OnSearchCommandPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(_search.SearchCommand.IsRunning))
        {
            SearchExample.Output = $"SearchCommand.IsRunning = {_search.SearchCommand.IsRunning}";
        }
    }
}
