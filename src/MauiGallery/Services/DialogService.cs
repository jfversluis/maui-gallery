namespace MauiGallery.Services;

/// <summary>Shows dialogs on the current page of the window the user is working in.</summary>
public sealed class DialogService(ActiveWindowTracker windows) : IDialogService
{
    public Task<bool> ConfirmAsync(string title, string message, string accept, string cancel) =>
        CurrentPage.DisplayAlertAsync(title, message, accept, cancel);

    private Page CurrentPage
    {
        get
        {
            var shell = windows.ActiveShell ?? throw new InvalidOperationException("There is no window to show a dialog in.");
            return shell.CurrentPage ?? shell;
        }
    }
}
