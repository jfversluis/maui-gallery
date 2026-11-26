namespace MauiGallery.Features.Samples;

/// <summary>Title, description, native mappings and links at the top of every sample page.</summary>
public partial class ControlPageHeader : ContentView
{
    /// <summary>Below this width only the current platform's native type shows, with the rest behind a disclosure.</summary>
    private const double CompactWidth = 560;

    private bool? _compact;
    private bool _showAllPlatforms;

    public ControlPageHeader() => InitializeComponent();

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        _compact = null;
        UpdateMappings();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        UpdateMappings();
    }

    private static string CurrentPlatformName() =>
        DeviceInfo.Platform == DevicePlatform.Android ? "Android"
        : DeviceInfo.Platform == DevicePlatform.MacCatalyst ? "macOS"
        : DeviceInfo.Platform == DevicePlatform.WinUI ? "Windows"
        : "iOS";

    private void UpdateMappings()
    {
        if (BindingContext is not SamplePageViewModel { HasNativeMappings: true } viewModel || Width <= 0)
        {
            return;
        }

        var compact = Width < CompactWidth;
        if (_compact == compact && !compact)
        {
            return;
        }

        _compact = compact;
        var current = viewModel.NativeMappings.FirstOrDefault(m => m.Platform == CurrentPlatformName());
        var others = viewModel.NativeMappings.Count - (current is null ? 0 : 1);
        var collapsible = compact && others > 0;

        CompactMappings.IsVisible = collapsible;
        CurrentMapping.Text = current?.Text ?? string.Empty;
        AllMappings.IsVisible = !collapsible || _showAllPlatforms;

        // Expanded, the full list above already includes the current platform.
        CurrentMapping.IsVisible = current is not null && !_showAllPlatforms;
        AllPlatformsButton.Text = _showAllPlatforms
            ? "Fewer platforms"
            : current is null ? $"Native types on {others} platforms" : "All platforms";
        SemanticProperties.SetHint(AllPlatformsButton, _showAllPlatforms ? "Shows only this platform's native type" : "Shows the native type on every platform");
    }

    private void OnAllPlatformsClicked(object? sender, EventArgs e)
    {
        _showAllPlatforms = !_showAllPlatforms;
        _compact = null;
        UpdateMappings();
    }
}
