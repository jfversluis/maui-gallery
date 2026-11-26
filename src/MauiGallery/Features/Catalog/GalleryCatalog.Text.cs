using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Controls.Text;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] TextItems() =>
    [
        new()
        {
            Id = "label",
            Title = "Label",
            Description = "Displays read-only text, from a single word to rich, formatted paragraphs.",
            CategoryId = CategoryIds.Text,
            IconGlyph = FluentIconsRegular.TextCaseTitle24,
            PageType = typeof(LabelPage),
            Keywords = ["text", "span", "formatted text", "html", "truncate", "ellipsis", "wrap", "link", "underline", "line height"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UILabel"),
                new(GalleryPlatforms.Android, "TextView"),
                new(GalleryPlatforms.MacCatalyst, "UILabel"),
                new(GalleryPlatforms.Windows, "TextBlock"),
            ],
            RelatedIds = ["typography", "entry", "editor"],
            DocsUrl = new(DocsRoot + "user-interface/controls/label"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.label"),
            SourceUrl = new(SourceRoot + "Controls/Text/Label"),
        },
        new()
        {
            Id = "entry",
            Title = "Entry",
            Description = "A single-line text field for names, email addresses, passwords and other short input.",
            CategoryId = CategoryIds.Text,
            IconGlyph = FluentIconsRegular.TextField24,
            PageType = typeof(EntryPage),
            Keywords = ["text box", "text field", "input", "textbox", "password", "keyboard", "placeholder", "validation", "form"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UITextField"),
                new(GalleryPlatforms.Android, "AppCompatEditText"),
                new(GalleryPlatforms.MacCatalyst, "UITextField"),
                new(GalleryPlatforms.Windows, "TextBox"),
            ],
            RelatedIds = ["editor", "search-bar", "label"],
            DocsUrl = new(DocsRoot + "user-interface/controls/entry"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.entry"),
            SourceUrl = new(SourceRoot + "Controls/Text/Entry"),
            IsFeatured = true,
        },
        new()
        {
            Id = "editor",
            Title = "Editor",
            Description = "A multi-line text field for longer input, such as comments and notes.",
            CategoryId = CategoryIds.Text,
            IconGlyph = FluentIconsRegular.Notepad24,
            PageType = typeof(EditorPage),
            Keywords = ["multiline", "text area", "textarea", "notes", "comment", "auto size", "max length"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UITextView"),
                new(GalleryPlatforms.Android, "AppCompatEditText"),
                new(GalleryPlatforms.MacCatalyst, "UITextView"),
                new(GalleryPlatforms.Windows, "TextBox"),
            ],
            RelatedIds = ["entry", "search-bar", "label"],
            DocsUrl = new(DocsRoot + "user-interface/controls/editor"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.editor"),
            SourceUrl = new(SourceRoot + "Controls/Text/Editor"),
        },
        new()
        {
            Id = "search-bar",
            Title = "SearchBar",
            Description = "A text field built for search, with a search icon, a cancel button and a search command.",
            CategoryId = CategoryIds.Text,
            IconGlyph = FluentIconsRegular.Search24,
            PageType = typeof(SearchBarPage),
            Keywords = ["search", "filter", "query", "find", "search command"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UISearchBar"),
                new(GalleryPlatforms.Android, "SearchView"),
                new(GalleryPlatforms.MacCatalyst, "UISearchBar"),
                new(GalleryPlatforms.Windows, "AutoSuggestBox"),
            ],
            RelatedIds = ["entry", "editor"],
            DocsUrl = new(DocsRoot + "user-interface/controls/searchbar"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.searchbar"),
            SourceUrl = new(SourceRoot + "Controls/Text/SearchBar"),
        },
    ];
}
