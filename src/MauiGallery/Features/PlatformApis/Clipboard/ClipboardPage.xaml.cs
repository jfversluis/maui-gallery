using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class ClipboardPage : SamplePage
{
    private readonly ClipboardViewModel _clipboard;

    public ClipboardPage(SamplePageServices services, ClipboardViewModel clipboard)
        : base(services)
    {
        InitializeComponent();

        _clipboard = clipboard;
        CopySample.BindingContext = clipboard;
        ChangesSample.BindingContext = clipboard;
        CopyExample.NativeTarget = CopyButton;
        ChangesExample.NativeTarget = ClearButton;

        clipboard.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ClipboardViewModel.ChangeCount))
            {
                ChangesExample.Output = $"ClipboardContentChanged, HasText = {clipboard.HasText}";
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _clipboard.Start();
    }

    protected override void OnDisappearing()
    {
        _clipboard.Stop();
        base.OnDisappearing();
    }

    private void OnCopyClicked(object? sender, EventArgs e) =>
        CopyExample.Output = $"SetTextAsync(\"{_clipboard.Text}\")";

    private void OnPasteClicked(object? sender, EventArgs e) =>
        CopyExample.Output = "GetTextAsync()";
}
