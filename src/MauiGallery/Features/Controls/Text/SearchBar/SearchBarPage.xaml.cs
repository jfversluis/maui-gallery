using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Text;

public sealed partial class SearchBarPage : SamplePage
{
    public SearchBarPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        CommandSample.BindingContext = new SearchBarCommandViewModel();
        CommandExample.NativeTarget = CommandSearchBar;
        FilterSample.BindingContext = new SearchBarFilterViewModel();
        FilterExample.NativeTarget = FilterSearchBar;
    }

    private void OnBasicSearchButtonPressed(object? sender, EventArgs e) =>
        BasicExample.Output = $"SearchButtonPressed: \"{BasicSearchBar.Text}\"";
}
