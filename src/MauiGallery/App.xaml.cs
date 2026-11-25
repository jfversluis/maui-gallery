using MauiGallery.Services;

namespace MauiGallery;

public partial class App : Application
{
    public App(IThemeService themeService)
    {
        InitializeComponent();
        themeService.Initialize(this);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var shell = new AppShell();
        var window = new Window(shell);
        window.SizeChanged += (_, _) => shell.ApplyMode(window.Width);
        return window;
    }
}
