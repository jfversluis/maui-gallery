using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.BasicInput;

public sealed partial class ButtonPage : SamplePage
{
    private int _clickCount;

    public ButtonPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        CommandSample.BindingContext = new ButtonCommandViewModel();
        CommandExample.NativeTarget = CommandButton;
        IconPosition.SelectedIndex = 0;
        OnIconLayoutChanged(this, EventArgs.Empty);
    }

    private void OnBasicButtonClicked(object? sender, EventArgs e)
    {
        _clickCount++;
        BasicExample.Output = _clickCount == 1 ? "Clicked once" : $"Clicked {_clickCount} times";
    }

    private void OnIconLayoutChanged(object? sender, EventArgs e)
    {
        var position = Enum.Parse<Button.ButtonContentLayout.ImagePosition>((string)IconPosition.SelectedItem ?? nameof(Button.ButtonContentLayout.ImagePosition.Left));
        var spacing = Math.Round(IconSpacing.Value);
        IconButton.ContentLayout = new Button.ButtonContentLayout(position, spacing);

        var isDefault = position == Button.ButtonContentLayout.ImagePosition.Left && spacing == 10;
        IconLayoutSubstitution.Value = isDefault ? null : $"{position},{spacing}";
        IconPositionSubstitution.Value = $"Button.ButtonContentLayout.ImagePosition.{position}";
        IconSpacingSubstitution.Value = spacing;
    }
}
