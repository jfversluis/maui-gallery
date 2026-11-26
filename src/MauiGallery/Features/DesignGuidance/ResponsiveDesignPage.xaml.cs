using MauiGallery.Core.Layout;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.DesignGuidance;

public sealed partial class ResponsiveDesignPage : SamplePage
{
    private Window? _window;

    public ResponsiveDesignPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        IdiomExample.Output = $"DeviceInfo.Idiom: {DeviceInfo.Idiom} · DeviceInfo.Platform: {DeviceInfo.Platform}";
    }

    private void OnLoaded(object? sender, EventArgs e)
    {
        _window = Window;
        if (_window is not null)
        {
            _window.SizeChanged += OnWindowSizeChanged;
        }

        UpdateReadout();
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        if (_window is not null)
        {
            _window.SizeChanged -= OnWindowSizeChanged;
            _window = null;
        }
    }

    private void OnWindowSizeChanged(object? sender, EventArgs e) => UpdateReadout();

    private void UpdateReadout()
    {
        if (_window is not { Width: > 0 } window)
        {
            return;
        }

        var sizeClass = Breakpoints.Classify(window.Width);
        WindowReadout.Text = $"{window.Width:0} × {window.Height:0}";
        SizeClassReadout.Text = sizeClass.ToString();
        ChromeReadout.Text = window.Page is Shell shell
            ? $"{DeviceInfo.Idiom} idiom · Shell FlyoutBehavior: {shell.FlyoutBehavior}"
            : $"{DeviceInfo.Idiom} idiom";
        StatesExample.Output = $"Window width {window.Width:0}: VisualState {sizeClass}";

        foreach (var (row, rowClass) in new[] { (CompactRow, SizeClass.Compact), (MediumRow, SizeClass.Medium), (ExpandedRow, SizeClass.Expanded) })
        {
            if (rowClass == sizeClass)
            {
                row.SetDynamicResource(BackgroundColorProperty, "ColorSelection");
            }
            else
            {
                row.BackgroundColor = Colors.Transparent;
            }
        }
    }
}
