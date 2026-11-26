namespace MauiGallery.Services;

/// <summary>
/// Shows dialogs on behalf of view models, which have no page to call <c>DisplayAlertAsync</c> on and should not
/// reference one.
/// </summary>
public interface IDialogService
{
    Task<bool> ConfirmAsync(string title, string message, string accept, string cancel);
}
