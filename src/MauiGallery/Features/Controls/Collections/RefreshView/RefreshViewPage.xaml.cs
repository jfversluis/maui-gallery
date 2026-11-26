using System.Globalization;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Collections;

public sealed partial class RefreshViewPage : SamplePage
{
    private const decimal Budget = 2000m;
    private static readonly CultureInfo Dollars = CultureInfo.GetCultureInfo("en-US");

    private decimal _spent = 1240m;

    public RefreshViewPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        var weather = new WeatherFeedViewModel();
        weather.Refreshed += (_, _) => ListExample.Output = $"RefreshCommand finished. {weather.LastUpdated}";
        ListSample.BindingContext = weather;
        ListExample.NativeTarget = ListRefresh;
    }

    private async void OnBudgetRefreshing(object? sender, EventArgs e)
    {
        EventExample.Output = "Refreshing event raised";

        // Stands in for a call to a banking service.
        await Task.Delay(1200);
        _spent = Math.Min(Budget, _spent + 35m + (DateTime.Now.Second % 5 * 20m));
        BudgetSpent.Text = $"{_spent.ToString("C0", Dollars)} of {Budget.ToString("C0", Dollars)}";
        BudgetProgress.Progress = (double)(_spent / Budget);
        BudgetUpdated.Text = $"Synced at {DateTime.Now:T}";

        BudgetRefresh.IsRefreshing = false;
        EventExample.Output = "Done: IsRefreshing set back to false";
    }

    private void OnBudgetRefreshClicked(object? sender, EventArgs e) => BudgetRefresh.IsRefreshing = true;
}
