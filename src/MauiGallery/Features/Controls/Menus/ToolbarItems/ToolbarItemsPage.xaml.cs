using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Menus;

public sealed partial class ToolbarItemsPage : SamplePage
{
    public ToolbarItemsPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        var note = new NoteViewModel();
        note.Saved += (_, text) => CommandExample.Output = $"SaveCommand executed: \"{text}\"";
        CommandNote.BindingContext = note;
        SaveItem.Command = note.SaveCommand;

        OnPrimaryOptionChanged(this, EventArgs.Empty);
    }

    private void OnPrimaryClicked(object? sender, EventArgs e) =>
        PrimaryExample.Output = $"Clicked: {((ToolbarItem)sender!).Text}";

    private void OnSecondaryClicked(object? sender, EventArgs e) =>
        SecondaryExample.Output = $"Clicked: {((ToolbarItem)sender!).Text}";

    private void OnPrimaryOptionChanged(object? sender, EventArgs e)
    {
        Show(AddItem, PrimaryShowAdd.IsToggled, index: 0);
        Show(ShareItem, PrimaryShowShare.IsToggled, index: ToolbarItems.Contains(AddItem) ? 1 : 0);

        // A null value drops the item's line from the snippet.
        PrimaryAddSubstitution.Value = PrimaryShowAdd.IsToggled ? "Add" : null;
        PrimaryShareSubstitution.Value = PrimaryShowShare.IsToggled ? "Share" : null;
    }

    private void OnSecondaryOptionChanged(object? sender, EventArgs e)
    {
        Show(RenameItem, SecondaryShow.IsToggled, ToolbarItems.Count);
        Show(DuplicateItem, SecondaryShow.IsToggled, ToolbarItems.Count);
        Show(PrintItem, SecondaryShow.IsToggled, ToolbarItems.Count);
    }

    private void Show(ToolbarItem item, bool show, int index)
    {
        var shown = ToolbarItems.Contains(item);
        if (show && !shown)
        {
            ToolbarItems.Insert(Math.Min(index, ToolbarItems.Count), item);
        }
        else if (!show && shown)
        {
            ToolbarItems.Remove(item);
        }
    }
}
