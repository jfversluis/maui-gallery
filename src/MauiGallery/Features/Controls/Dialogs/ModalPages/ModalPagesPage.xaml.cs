using MauiGallery.Features.Samples;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;
using iOSPage = Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific.Page;

namespace MauiGallery.Features.Controls.Dialogs;

public sealed partial class ModalPagesPage : SamplePage
{
    public ModalPagesPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();

        StylePicker.ItemsSource = new[]
        {
            UIModalPresentationStyle.FullScreen,
            UIModalPresentationStyle.PageSheet,
            UIModalPresentationStyle.FormSheet,
            UIModalPresentationStyle.OverFullScreen,
            UIModalPresentationStyle.Automatic,
        };
        StylePicker.SelectedIndex = 0;
        ResultExample.NativeTarget = ResultButton;
    }

    private async void OnBasicClicked(object? sender, EventArgs e)
    {
        var page = new WhatsNewPage();
        page.Disappearing += (_, _) => BasicExample.Output = "PopModalAsync: the modal page closed";
        await Navigation.PushModalAsync(page, BasicAnimated.IsToggled);
        BasicExample.Output = "PushModalAsync: the modal page is showing";
    }

    private async void OnStyleClicked(object? sender, EventArgs e)
    {
        var style = (UIModalPresentationStyle)StylePicker.SelectedItem;
        var page = new WhatsNewPage();
        iOSPage.SetModalPresentationStyle(page.On<iOS>(), style);
        page.Disappearing += (_, _) => StyleExample.Output = $"Closed the {style} modal page";
        await Navigation.PushModalAsync(page);
        StyleExample.Output = $"Showing as {style}";
    }

    private async void OnResultClicked(object? sender, EventArgs e)
    {
        var form = new ContactFormPage();
        await Navigation.PushModalAsync(form);

        var contact = await form.Result;
        if (contact is null)
        {
            ResultExample.Output = "Result: null (cancelled)";
            return;
        }

        ResultSummary.Text = $"{contact.Name} · {contact.Email}";
        ResultExample.Output = $"Result: {contact}";
    }
}
