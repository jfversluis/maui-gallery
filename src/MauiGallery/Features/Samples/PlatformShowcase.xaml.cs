using MauiGallery.Core.Catalog;

namespace MauiGallery.Features.Samples;

/// <summary>
/// "Same code, every platform": screenshots of the page's first example on iOS, Android, macOS and Windows, each
/// labeled with the native control MAUI created. The tiles and images load after the page has appeared, so they never
/// slow it down, and follow the app's light or dark theme.
/// </summary>
public sealed partial class PlatformShowcase : ContentView
{
    /// <summary>The captures are stored at two pixels per device-independent unit (see eng/platform-shots).</summary>
    private const double ImageDensity = 2;
    private const double PlaceholderWellHeight = 120;
    private const double MinimumWellHeight = 96;
    private const double MaximumWellHeight = 220;

    /// <summary>Matches the image area's padding and the tile's stroke in PlatformShowcase.xaml.</summary>
    private const double WellPadding = 12;
    private const double TileStroke = 1.5;
    private static readonly TimeSpan LoadDelay = TimeSpan.FromMilliseconds(400);

    private IReadOnlyList<PlatformShotTile> _tiles = [];
    private GalleryItem? _item;
    private Application? _application;
    private bool? _loadedDark;
    private bool? _loadingDark;
    private int _loadVersion;

    public PlatformShowcase()
    {
        InitializeComponent();
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        Tiles.SizeChanged += OnTilesSizeChanged;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        if (BindingContext is not SamplePageViewModel { Item.HasPlatformShots: true } viewModel || viewModel.Item == _item)
        {
            return;
        }

        _item = viewModel.Item;
        _loadedDark = _loadingDark = null;
        var current = CurrentPlatform();
        _tiles = [.. PlatformShots.Platforms.Select(platform => new PlatformShotTile(
            _item.Title,
            platform,
            NativeMappingDisplay.PlatformName(platform),
            _item.NativeMappings.FirstOrDefault(m => m.Platform == platform)?.NativeType,
            platform == current)
        {
            WellHeight = PlaceholderWellHeight,
        })];
        Intro.Text = $"The first example above, captured on each platform: the same {_item.Title} in your code becomes " +
            $"each platform's own native control. You're on {NativeMappingDisplay.PlatformName(current)}.";
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        _application = Application.Current;
        if (_application is not null)
        {
            _application.RequestedThemeChanged += OnRequestedThemeChanged;
        }

        // Let the page draw its first frame before reading any image.
        await Task.Delay(LoadDelay);
        await LoadAsync();
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        if (_application is not null)
        {
            _application.RequestedThemeChanged -= OnRequestedThemeChanged;
            _application = null;
        }

        // Abandon a load in progress; the next Loaded starts it again.
        _loadVersion++;
        _loadingDark = null;
    }

    private async void OnRequestedThemeChanged(object? sender, AppThemeChangedEventArgs e) => await LoadAsync();

    private async Task LoadAsync()
    {
        if (_item is null || !IsLoaded)
        {
            return;
        }

        var dark = (_application ?? Application.Current)?.RequestedTheme == AppTheme.Dark;
        if (_loadedDark == dark || _loadingDark == dark)
        {
            return;
        }

        _loadingDark = dark;
        var version = ++_loadVersion;

        // The tiles are created here rather than with the page, so they add nothing to its first frame.
        if (BindableLayout.GetItemsSource(Tiles) != _tiles)
        {
            BindableLayout.SetItemsSource(Tiles, _tiles);
        }

        var item = _item;
        var images = await Task.WhenAll(_tiles.Select(tile => ReadAsync(PlatformShots.PathFor(item.Id, tile.Platform, dark))));
        if (version != _loadVersion)
        {
            return;
        }

        _loadingDark = null;
        _loadedDark = dark;

        for (var i = 0; i < _tiles.Count; i++)
        {
            var tile = _tiles[i];
            var bytes = images[i];
            if (bytes is null || !PlatformShots.TryReadPngSize(bytes, out var width, out var height))
            {
                tile.IsMissing = true;
                tile.Image = null;
                continue;
            }

            tile.IsMissing = false;
            tile.NaturalSize = new Size(width / ImageDensity, height / ImageDensity);
            tile.Image = ImageSource.FromStream(() => new MemoryStream(bytes));
        }

        UpdateImageSizes();
    }

    private void OnTilesSizeChanged(object? sender, EventArgs e) => UpdateImageSizes();

    /// <summary>
    /// Shows the screenshots at the size each platform drew them, scaled down together when a tile is too narrow, so
    /// the four stay comparable.
    /// </summary>
    private void UpdateImageSizes()
    {
        var loaded = _tiles.Where(t => t.NaturalSize.Width > 0).ToList();
        if (loaded.Count == 0 || Tiles.Width <= 0)
        {
            return;
        }

        var columns = Tiles.ColumnsFor(Tiles.Width, _tiles.Count);
        var tileWidth = (Tiles.Width - ((columns - 1) * Tiles.ColumnSpacing)) / columns;
        var available = tileWidth - (2 * (WellPadding + TileStroke));
        var scale = Math.Min(1, available / loaded.Max(t => t.NaturalSize.Width));
        scale = Math.Min(scale, (MaximumWellHeight - (2 * WellPadding)) / loaded.Max(t => t.NaturalSize.Height));

        foreach (var tile in _tiles)
        {
            tile.DisplayWidth = Math.Floor(tile.NaturalSize.Width * scale);
            tile.DisplayHeight = Math.Floor(tile.NaturalSize.Height * scale);
        }

        var wellHeight = Math.Clamp(_tiles.Max(t => t.DisplayHeight) + (2 * WellPadding), MinimumWellHeight, MaximumWellHeight);
        foreach (var tile in _tiles)
        {
            tile.WellHeight = wellHeight;
        }
    }

    private static async Task<byte[]?> ReadAsync(string path)
    {
        // A missing image shows as "Not captured yet"; the repository tests make sure none ships. Checking first
        // avoids platform-specific exceptions (Windows throws DirectoryNotFoundException, Android a Java exception).
        if (!await FileSystem.Current.AppPackageFileExistsAsync(path))
        {
            return null;
        }

        await using var stream = await FileSystem.Current.OpenAppPackageFileAsync(path);
        using var buffer = new MemoryStream();
        await stream.CopyToAsync(buffer);
        return buffer.ToArray();
    }

    private static GalleryPlatforms CurrentPlatform() =>
        DeviceInfo.Platform == DevicePlatform.Android ? GalleryPlatforms.Android
        : DeviceInfo.Platform == DevicePlatform.MacCatalyst ? GalleryPlatforms.MacCatalyst
        : DeviceInfo.Platform == DevicePlatform.WinUI ? GalleryPlatforms.Windows
        : GalleryPlatforms.iOS;
}
