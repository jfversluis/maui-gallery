using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Media;

public sealed partial class BlazorWebViewPage : SamplePage
{
    private readonly PackingListViewModel _packingList;

    public BlazorWebViewPage(SamplePageServices services, PackingListViewModel packingList)
        : base(services)
    {
        _packingList = packingList;
        InitializeComponent();

        SharedSample.BindingContext = packingList;
        SharedExample.NativeTarget = SharedWebView;
        CounterWebView.BlazorWebViewInitialized += (_, _) => CounterExample.Output = "BlazorWebViewInitialized";

        // The view model is a singleton: only listen while the page is on screen.
        Loaded += (_, _) => _packingList.Changed += OnPackingListChanged;
        Unloaded += (_, _) => _packingList.Changed -= OnPackingListChanged;
        ConfigurePlatform();
    }

    /// <summary>Platform workarounds, applied before the web views are created (see Platforms/Windows).</summary>
    partial void ConfigurePlatform();

    private void OnPackingListChanged(object? sender, EventArgs e) =>
        SharedExample.Output = $"PackingListViewModel changed: {_packingList.Summary}";
}
