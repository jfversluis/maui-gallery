using MauiGallery.Features.Samples;
using MauiGallery.Services;

namespace MauiGallery.Features.Controls.Dialogs;

public sealed partial class AlertsPage : SamplePage
{
    public AlertsPage(SamplePageServices services, IDialogService dialogs)
        : base(services)
    {
        InitializeComponent();

        QuestionFlow.ItemsSource = new[] { FlowDirection.MatchParent, FlowDirection.LeftToRight, FlowDirection.RightToLeft };
        QuestionFlow.SelectedIndex = 0;

        var drafts = new DraftsViewModel(dialogs);
        drafts.Confirmed += (_, accepted) => ViewModelExample.Output = $"ConfirmAsync returned {accepted}";
        DraftsSample.BindingContext = drafts;
        DraftsRestore.Command = drafts.RestoreCommand;
        ViewModelExample.NativeTarget = DraftsButton;
    }

    private async void OnMessageClicked(object? sender, EventArgs e)
    {
        await DisplayAlertAsync(MessageTitle.Text, MessageText.Text, MessageCancel.Text);
        MessageExample.Output = "Alert dismissed";
    }

    private async void OnQuestionClicked(object? sender, EventArgs e)
    {
        var flowDirection = (FlowDirection)QuestionFlow.SelectedItem;
        var delete = await DisplayAlertAsync(
            "Delete this photo?",
            "It will be removed from all your devices.",
            QuestionAccept.Text,
            QuestionCancel.Text,
            flowDirection);
        QuestionExample.Output = $"DisplayAlertAsync returned {delete}";
    }

    private void OnQuestionFlowChanged(object? sender, EventArgs e) =>
        QuestionFlowSubstitution.Value = $"FlowDirection.{QuestionFlow.SelectedItem}";
}
