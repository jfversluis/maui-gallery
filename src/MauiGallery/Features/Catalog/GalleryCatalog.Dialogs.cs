using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Controls.Dialogs;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] DialogsItems() =>
    [
        new()
        {
            Id = "alerts",
            Title = "Alerts",
            Description = "Tells the user something important or asks a yes-or-no question with DisplayAlertAsync.",
            CategoryId = CategoryIds.Dialogs,
            IconGlyph = FluentIconsRegular.ChatWarning24,
            PageType = typeof(AlertsPage),
            Keywords = ["alert", "dialog", "DisplayAlertAsync", "confirm", "message box", "popup", "question", "yes", "no"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIAlertController (alert)"),
                new(GalleryPlatforms.Android, "AlertDialog"),
                new(GalleryPlatforms.MacCatalyst, "UIAlertController (alert)"),
                new(GalleryPlatforms.Windows, "ContentDialog"),
            ],
            RelatedIds = ["action-sheets", "prompts", "modal-pages"],
            DocsUrl = new(DocsRoot + "user-interface/pop-ups"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.page.displayalertasync"),
            SourceUrl = new(SourceRoot + "Controls/Dialogs/Alerts"),
        },
        new()
        {
            Id = "action-sheets",
            Title = "Action sheets",
            Description = "Offers a set of choices for the task at hand, including cancel and destructive actions.",
            CategoryId = CategoryIds.Dialogs,
            IconGlyph = FluentIconsRegular.ListBar24,
            PageType = typeof(ActionSheetsPage),
            Keywords = ["action sheet", "DisplayActionSheetAsync", "choices", "options", "menu", "popover", "destructive", "dialog"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIAlertController (action sheet)"),
                new(GalleryPlatforms.Android, "AlertDialog (list)"),
                new(GalleryPlatforms.MacCatalyst, "UIAlertController (action sheet)"),
                new(GalleryPlatforms.Windows, "ContentDialog"),
            ],
            RelatedIds = ["alerts", "prompts", "context-menu"],
            DocsUrl = new(DocsRoot + "user-interface/pop-ups#guide-users-through-tasks"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.page.displayactionsheetasync"),
            SourceUrl = new(SourceRoot + "Controls/Dialogs/ActionSheets"),
        },
        new()
        {
            Id = "prompts",
            Title = "Prompts",
            Description = "Asks the user to type a short answer, such as a name or a code, with DisplayPromptAsync.",
            CategoryId = CategoryIds.Dialogs,
            IconGlyph = FluentIconsRegular.TextboxMore24,
            PageType = typeof(PromptsPage),
            Keywords = ["prompt", "DisplayPromptAsync", "input", "text", "dialog", "rename", "keyboard", "question"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIAlertController + UITextField"),
                new(GalleryPlatforms.Android, "AlertDialog + EditText"),
                new(GalleryPlatforms.MacCatalyst, "UIAlertController + UITextField"),
                new(GalleryPlatforms.Windows, "ContentDialog + TextBox"),
            ],
            RelatedIds = ["alerts", "action-sheets", "entry"],
            DocsUrl = new(DocsRoot + "user-interface/pop-ups#display-a-prompt"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.page.displaypromptasync"),
            SourceUrl = new(SourceRoot + "Controls/Dialogs/Prompts"),
        },
        new()
        {
            Id = "modal-pages",
            Title = "Modal pages",
            Description = "Shows a page on top of the app for a focused task, and returns its result to the caller.",
            CategoryId = CategoryIds.Dialogs,
            IconGlyph = FluentIconsRegular.WindowArrowUp24,
            PageType = typeof(ModalPagesPage),
            Keywords = ["modal", "PushModalAsync", "PopModalAsync", "sheet", "form", "full screen", "page sheet", "ModalPresentationStyle", "dialog"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIViewController (presented)"),
                new(GalleryPlatforms.Android, "DialogFragment"),
                new(GalleryPlatforms.MacCatalyst, "UIViewController (presented)"),
                new(GalleryPlatforms.Windows, "Frame overlay"),
            ],
            RelatedIds = ["alerts", "prompts"],
            DocsUrl = new(DocsRoot + "user-interface/pages/navigationpage#perform-modal-navigation"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.inavigation.pushmodalasync"),
            SourceUrl = new(SourceRoot + "Controls/Dialogs/ModalPages"),
        },
    ];
}
