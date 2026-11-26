using MauiGallery.Services;

namespace MauiGallery;

public partial class App : Application
{
    private readonly GalleryWindowFactory _windows;

    public App(IThemeService themeService, GalleryWindowFactory windows)
    {
        InitializeComponent();
        _windows = windows;
        themeService.Initialize(this);
        PageLoadTrace.Mark("app");
    }

    protected override Window CreateWindow(IActivationState? activationState) => _windows.Create();
}
