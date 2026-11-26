using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class MediaPickerPage : SamplePage
{
    private readonly MediaPickerViewModel _viewModel;

    public MediaPickerPage(SamplePageServices services, MediaPickerViewModel viewModel)
        : base(services)
    {
        _viewModel = viewModel;
        InitializeComponent();
        PickSample.BindingContext = viewModel;
        PickOptions.BindingContext = viewModel;
        PickManySample.BindingContext = viewModel;
        PickManyOptions.BindingContext = viewModel;
        CaptureSample.BindingContext = viewModel;
        VideoSample.BindingContext = viewModel;
        PickExample.NativeTarget = PickButton;
        PickManyExample.NativeTarget = PickManyButton;
        CaptureExample.NativeTarget = CaptureButton;
        VideoExample.NativeTarget = VideoButton;

        viewModel.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(MediaPickerViewModel.PhotoInfo):
                    PickExample.Output = $"PickPhotoAsync: {viewModel.PhotoInfo}";
                    break;
                case nameof(MediaPickerViewModel.PhotosInfo):
                    PickManyExample.Output = $"PickPhotosAsync: {viewModel.PhotosInfo}";
                    break;
                case nameof(MediaPickerViewModel.CaptureInfo):
                    CaptureExample.Output = $"CapturePhotoAsync: {viewModel.CaptureInfo}";
                    break;
                case nameof(MediaPickerViewModel.VideoInfo):
                    VideoExample.Output = viewModel.VideoInfo;
                    break;
            }
        };
    }

    private void OnPickSizeChanged(object? sender, EventArgs e)
    {
        _viewModel.MaximumSize = int.TryParse(PickSize.SelectedItem as string, out var size) ? size : null;
        PickSizeSubstitution.Value = _viewModel.MaximumSize;
    }
}
