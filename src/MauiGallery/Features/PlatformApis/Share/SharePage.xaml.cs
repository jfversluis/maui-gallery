using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class SharePage : SamplePage
{
    private readonly IShare _share;
    private readonly IFileSystem _fileSystem;

    public SharePage(SamplePageServices services, IShare share, IFileSystem fileSystem)
        : base(services)
    {
        _share = share;
        _fileSystem = fileSystem;
        InitializeComponent();
        FileExample.NativeTarget = FileButton;
    }

    private async void OnShareTextClicked(object? sender, EventArgs e)
    {
        await _share.RequestAsync(new ShareTextRequest
        {
            Text = NullIfEmpty(TextText.Text),
            Uri = NullIfEmpty(TextUri.Text),
            Title = NullIfEmpty(TextTitle.Text),
            PresentationSourceBounds = TextButton.GetBoundsInWindow(),
        });
        TextExample.Output = "Share sheet closed";
    }

    private async void OnShareFileClicked(object? sender, EventArgs e)
    {
        var name = string.IsNullOrWhiteSpace(FileName.Text) ? "shared.txt" : Path.GetFileName(FileName.Text.Trim());
        var path = Path.Combine(_fileSystem.CacheDirectory, name);
        await File.WriteAllTextAsync(path, FileContents.Text);

        await _share.RequestAsync(new ShareFileRequest
        {
            Title = NullIfEmpty(FileTitle.Text),
            File = new ShareFile(path),
            PresentationSourceBounds = FileButton.GetBoundsInWindow(),
        });
        FileExample.Output = $"Shared {name} ({new FileInfo(path).Length} bytes)";
    }

    private static string? NullIfEmpty(string? text) => string.IsNullOrWhiteSpace(text) ? null : text;
}
