using MauiGallery.Core.Snippets;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.Dialogs;

public sealed partial class ActionSheetsPage : SamplePage
{
    public ActionSheetsPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        DestructiveFlow.ItemsSource = new[] { FlowDirection.MatchParent, FlowDirection.LeftToRight, FlowDirection.RightToLeft };
        DestructiveFlow.SelectedIndex = 0;
        OnChoiceButtonsChanged(this, EventArgs.Empty);
        OnDestructiveOptionChanged(this, EventArgs.Empty);
    }

    private string[] Choices =>
        (ChoiceButtons.Text ?? string.Empty).Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

    private async void OnChoiceClicked(object? sender, EventArgs e)
    {
        var choice = await DisplayActionSheetAsync(ChoiceTitle.Text, "Cancel", null, Choices);
        ChoiceExample.Output = Describe(choice);
    }

    private async void OnDestructiveClicked(object? sender, EventArgs e)
    {
        var action = await DisplayActionSheetAsync(
            "What do you want to do with this photo?",
            NullIfEmpty(DestructiveCancel.Text),
            NullIfEmpty(DestructiveDestruction.Text),
            (FlowDirection)DestructiveFlow.SelectedItem,
            "Share", "Duplicate");
        DestructiveExample.Output = Describe(action);
    }

    private void OnChoiceButtonsChanged(object? sender, EventArgs e) =>
        ChoiceButtonsSubstitution.Value = string.Join(", ", Choices.Select(SnippetValueFormatter.FormatCSharp));

    private void OnDestructiveOptionChanged(object? sender, EventArgs e)
    {
        if (DestructiveFlow is null || DestructiveFlowSubstitution is null)
        {
            return;
        }

        DestructiveCancelSubstitution.Value = SnippetValueFormatter.FormatCSharp(NullIfEmpty(DestructiveCancel.Text));
        DestructiveDestructionSubstitution.Value = SnippetValueFormatter.FormatCSharp(NullIfEmpty(DestructiveDestruction.Text));
        DestructiveFlowSubstitution.Value = $"FlowDirection.{DestructiveFlow.SelectedItem}";
    }

    private static string? NullIfEmpty(string? text) => string.IsNullOrWhiteSpace(text) ? null : text;

    private static string Describe(string? result) =>
        result is null ? "DisplayActionSheetAsync returned null" : $"DisplayActionSheetAsync returned \"{result}\"";
}
