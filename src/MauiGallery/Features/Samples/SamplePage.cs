using MauiGallery.Services;

namespace MauiGallery.Features.Samples;

/// <summary>
/// Base class for every gallery page. Looks up the page's catalog entry and exposes a
/// <see cref="SamplePageViewModel"/> for the header, favorite and related-items UI.
/// </summary>
public class SamplePage : ContentPage
{
    private readonly int _navigation;

    public SamplePage(SamplePageServices services)
    {
        ArgumentNullException.ThrowIfNull(services);
        _navigation = PageLoadTrace.Navigation;
        PageLoadTrace.Mark("resolve");

        var item = services.Catalog.FindByPage(GetType())
            ?? throw new InvalidOperationException($"{GetType().Name} has no entry in the gallery catalog.");
        ViewModel = new SamplePageViewModel(item, services);
        BindingContext = ViewModel;
        Title = item.Title;
        Shell.SetNavBarIsVisible(this, true);
        SetDynamicResource(BackgroundColorProperty, "ColorBackground");
    }

    protected SamplePageViewModel ViewModel { get; }

    protected override void OnParentSet()
    {
        base.OnParentSet();
        MarkLoad("construct", Parent is not null);
    }

    protected override void OnHandlerChanged()
    {
        base.OnHandlerChanged();
        MarkLoad("handlers", Handler is not null);
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        MarkLoad("appear", true);
        ViewModel.OnAppearing();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        MarkLoad("layout", width > 0);
    }

    private void MarkLoad(string milestone, bool condition)
    {
        if (condition && _navigation == PageLoadTrace.Navigation)
        {
            PageLoadTrace.Mark(milestone);
        }
    }
}
