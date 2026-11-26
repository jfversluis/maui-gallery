namespace MauiGallery.Features.Navigation;

/// <summary>Reports a window's lifecycle events, by name, until it is disposed.</summary>
internal sealed class WindowEventLog : IDisposable
{
    private readonly Window _window;
    private readonly string _name;
    private readonly Action<string> _log;

    public WindowEventLog(Window window, string name, Action<string> log)
    {
        _window = window;
        _name = name;
        _log = log;

        window.Created += OnCreated;
        window.Activated += OnActivated;
        window.Deactivated += OnDeactivated;
        window.Stopped += OnStopped;
        window.Resumed += OnResumed;
        window.Destroying += OnDestroying;
    }

    public void Dispose()
    {
        _window.Created -= OnCreated;
        _window.Activated -= OnActivated;
        _window.Deactivated -= OnDeactivated;
        _window.Stopped -= OnStopped;
        _window.Resumed -= OnResumed;
        _window.Destroying -= OnDestroying;
    }

    private void OnCreated(object? sender, EventArgs e) => _log($"{_name}: Created");

    private void OnActivated(object? sender, EventArgs e) => _log($"{_name}: Activated");

    private void OnDeactivated(object? sender, EventArgs e) => _log($"{_name}: Deactivated");

    private void OnStopped(object? sender, EventArgs e) => _log($"{_name}: Stopped");

    private void OnResumed(object? sender, EventArgs e) => _log($"{_name}: Resumed");

    private void OnDestroying(object? sender, EventArgs e) => _log($"{_name}: Destroying");
}
