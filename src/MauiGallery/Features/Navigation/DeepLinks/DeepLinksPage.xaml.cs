using MauiGallery.Core.Catalog;
using MauiGallery.Core.Navigation;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Navigation;

public sealed partial class DeepLinksPage : SamplePage
{
    private const string CommandItemId = "button";

    private static readonly string[] ItemIds = ["button", "collection-view", "shell", "animations"];

    private readonly IClipboard _clipboard;

    public DeepLinksPage(SamplePageServices services)
        : base(services)
    {
        ArgumentNullException.ThrowIfNull(services);
        InitializeComponent();
        _clipboard = services.Clipboard;
        CopyExample.NativeTarget = CopyButton;
        ReceiveExample.NativeTarget = ReceiveCopy;

        var items = ItemIds.Select(services.Catalog.Find).OfType<GalleryItem>().ToList();
        CopyPicker.ItemsSource = items;
        OpenPicker.ItemsSource = items;
        CopyPicker.SelectedIndexChanged += (_, _) => UpdateCopyLink();
        OpenPicker.SelectedIndexChanged += (_, _) => OpenLinkSubstitution.Value = LinkFor(OpenPicker)?.ToString();
        CopyPicker.SelectedIndex = 0;
        OpenPicker.SelectedIndex = 0;

        var link = DeepLink.ForItem(CommandItemId);
        (ReceiveIntro.Text, ReceiveCommand.Text) = DeviceInfo.Platform switch
        {
            var p when p == DevicePlatform.iOS => ("Send this simulator a link from a Mac terminal:", $"xcrun simctl openurl booted {link}"),
            var p when p == DevicePlatform.MacCatalyst => ("Open a link from Terminal:", $"open {link}"),
            var p when p == DevicePlatform.Android => ("Send the emulator a link from a terminal:", $"adb shell am start -a android.intent.action.VIEW -d \"{link}\""),
            _ => ("This gallery runs unpackaged on Windows, so it doesn't register the scheme there. A packaged app opens links from a command prompt with:", $"start {link}"),
        };
    }

    private static Uri? LinkFor(Picker picker) => picker.SelectedItem is GalleryItem item ? DeepLink.ForItem(item.Id) : null;

    private void UpdateCopyLink()
    {
        CopyLink.Text = LinkFor(CopyPicker)?.ToString();
        CopyIdSubstitution.Value = (CopyPicker.SelectedItem as GalleryItem)?.Id;
    }

    private async void OnCopyClicked(object? sender, EventArgs e)
    {
        if (LinkFor(CopyPicker) is { } link)
        {
            await _clipboard.SetTextAsync(link.ToString());
            CopyExample.Output = $"Copied {link}";
        }
    }

    private async void OnOpenClicked(object? sender, EventArgs e)
    {
        if (LinkFor(OpenPicker) is not { } link)
        {
            return;
        }

        var opened = await Launcher.Default.OpenAsync(link);
        OpenExample.Output = $"Launcher.OpenAsync(\"{link}\") returned {opened}";
    }

    private async void OnCopyCommandClicked(object? sender, EventArgs e)
    {
        await _clipboard.SetTextAsync(ReceiveCommand.Text);
        ReceiveExample.Output = "Copied the command";
    }
}
