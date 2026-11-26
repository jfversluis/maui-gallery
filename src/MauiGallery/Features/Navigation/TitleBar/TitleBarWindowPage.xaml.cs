namespace MauiGallery.Features.Navigation;

/// <summary>The page in the window that the title bar sample opens.</summary>
public sealed partial class TitleBarWindowPage : ContentPage
{
    public TitleBarWindowPage() => InitializeComponent();

    /// <summary>Shows what the controls in the title bar did.</summary>
    public void ShowStatus(string text) => Status.Text = text;

    private void OnCloseClicked(object? sender, EventArgs e)
    {
        if (Window is { } window)
        {
            Application.Current?.CloseWindow(window);
        }
    }
}
