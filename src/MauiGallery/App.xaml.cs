using MauiGallery.Services;

namespace MauiGallery;

public partial class App : Application
{
    private readonly IServiceProvider _services;

    public App(IThemeService themeService, IServiceProvider services)
    {
        InitializeComponent();
        _services = services;
        themeService.Initialize(this);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // Each window gets its own Shell (and sidebar), so multiple windows navigate independently.
        var shell = _services.GetRequiredService<AppShell>();
        var window = new Window(shell) { Title = ".NET MAUI Gallery" };
        window.SizeChanged += (_, _) => shell.ApplyLayout(window.Width);
        return window;
    }
}
