namespace MauiGallery.Features.Catalog;

public sealed partial class AllControlsPage : ContentPage
{
    public AllControlsPage(AllControlsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
