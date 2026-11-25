namespace MauiGallery.Services;

/// <summary>
/// Remembers which window the user last activated, so navigation targets that window's Shell even when several
/// windows are open. <c>Shell.Current</c> is ambiguous with multiple windows.
/// </summary>
public sealed class ActiveWindowTracker
{
    private WeakReference<Window>? _active;

    public Shell? ActiveShell
    {
        get
        {
            if (_active is not null && _active.TryGetTarget(out var window))
            {
                return window.Page as Shell;
            }

            return Application.Current?.Windows is { Count: > 0 } all ? all[0].Page as Shell : null;
        }
    }

    public void Track(Window window)
    {
        ArgumentNullException.ThrowIfNull(window);

        _active = new WeakReference<Window>(window);
        window.Activated += (_, _) => _active = new WeakReference<Window>(window);
        window.Destroying += (_, _) =>
        {
            if (_active is not null && _active.TryGetTarget(out var active) && active == window)
            {
                _active = null;
            }
        };
    }
}
