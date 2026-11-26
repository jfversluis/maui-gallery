namespace MauiGallery.Features.Navigation;

/// <summary>The page in the small window that the multi-window sample opens. Its binding context is its own window.</summary>
public sealed partial class NotesWindowPage : ContentPage
{
    public NotesWindowPage() => InitializeComponent();

    private void OnBiggerClicked(object? sender, EventArgs e)
    {
        if (Window is { } window)
        {
            window.Width += 100;
            window.Height += 60;
        }
    }

    private void OnMoveClicked(object? sender, EventArgs e)
    {
        if (Window is { } window)
        {
            window.X = 40;
            window.Y = 40;
        }
    }

    private void OnCloseClicked(object? sender, EventArgs e)
    {
        if (Window is { } window)
        {
            Application.Current?.CloseWindow(window);
        }
    }
}
