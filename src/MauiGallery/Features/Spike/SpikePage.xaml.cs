namespace MauiGallery.Features.Spike;

public partial class SpikePage : ContentPage
{
    public SpikePage() => InitializeComponent();

    private void OnClicked(object? sender, EventArgs e) =>
        NativeTypeLabel.Text = NativeButton.Handler?.PlatformView?.GetType().FullName;
}
