using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class FilePickerPage : SamplePage
{
    private readonly IFileSystem _fileSystem;
    private readonly ILauncher _launcher;
    private string? _savedPath;

    public FilePickerPage(SamplePageServices services, FilePickerViewModel picker, IFileSystem fileSystem, ILauncher launcher)
        : base(services)
    {
        _fileSystem = fileSystem;
        _launcher = launcher;
        InitializeComponent();

        PickSample.BindingContext = picker;
        PickTypeOptions.BindingContext = picker;
        MultipleSample.BindingContext = picker;
        PickExample.NativeTarget = PickButton;
        MultipleExample.NativeTarget = MultipleButton;
        SaveExample.NativeTarget = SaveButton;
        UpdateFileTypesSubstitution(picker.FileTypeName);

        picker.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(FilePickerViewModel.FileTypeName):
                    UpdateFileTypesSubstitution(picker.FileTypeName);
                    break;
                case nameof(FilePickerViewModel.Status):
                    PickExample.Output = picker.Status == "Picked" ? $"PickAsync returned {picker.FileName}" : picker.Status;
                    break;
            }
        };
        picker.PickedFiles.CollectionChanged += (_, _) =>
            MultipleExample.Output = $"PickMultipleAsync returned {picker.PickedFiles.Count} files";
    }

    private void UpdateFileTypesSubstitution(string fileTypeName) =>
        FileTypesSubstitution.Value = fileTypeName switch
        {
            "Images" => "FilePickerFileType.Images",
            "PDF" => "FilePickerFileType.Pdf",
            "Text" => "TextFiles",
            _ => null,
        };

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        var name = string.IsNullOrWhiteSpace(SaveName.Text) ? "notes.txt" : Path.GetFileName(SaveName.Text.Trim());
        _savedPath = Path.Combine(_fileSystem.AppDataDirectory, name);
        await File.WriteAllTextAsync(_savedPath, SaveContents.Text);

        OpenButton.IsEnabled = true;
        SaveStatus.Text = $"Saved {name} to AppDataDirectory";
        SaveExample.Output = $"File.WriteAllTextAsync(\"{_savedPath}\")";
    }

    private async void OnOpenClicked(object? sender, EventArgs e)
    {
        if (_savedPath is null)
        {
            return;
        }

        var opened = await _launcher.OpenAsync(new OpenFileRequest
        {
            Title = "Open the notes",
            File = new ReadOnlyFile(_savedPath, "text/plain"),
            PresentationSourceBounds = OpenButton.GetBoundsInWindow(),
        });
        SaveExample.Output = $"Launcher.OpenAsync returned {opened}";
    }
}
