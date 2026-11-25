namespace MauiGallery.Features.Spike;

public partial class SpikePage : ContentPage
{
    public SpikePage() => InitializeComponent();

    private void OnLight(object? sender, EventArgs e) => Application.Current!.UserAppTheme = AppTheme.Light;

    private void OnDark(object? sender, EventArgs e) => Application.Current!.UserAppTheme = AppTheme.Dark;

    private void OnClicked(object? sender, EventArgs e) =>
        NativeTypeLabel.Text = NativeButton.Handler?.PlatformView?.GetType().FullName;
}
