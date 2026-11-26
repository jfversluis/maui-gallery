using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.BasicInput;

public sealed partial class ImageButtonPage : SamplePage
{
    private int _clickCount;

    public ImageButtonPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        CommandSample.BindingContext = new ImageButtonReactionsViewModel();
        CommandExample.NativeTarget = LikeButton;
    }

    private void OnBasicClicked(object? sender, EventArgs e)
    {
        _clickCount++;
        BasicExample.Output = _clickCount == 1 ? "Clicked once" : $"Clicked {_clickCount} times";
    }
}
