using MauiGallery.Core.Catalog;
using MauiGallery.Features.Samples;
using MauiGallery.Services;

namespace MauiGallery.Features.Navigation;

public sealed partial class MultiWindowPage : SamplePage
{
    private const int MaxLogLines = 8;

    private static readonly string[] ItemIds = ["button", "collection-view", "animations", "shell"];

    private readonly IWindowService _windows;
    private readonly List<Window> _openedWindows = [];
    private readonly List<string> _log = [];
    private readonly Dictionary<Window, WindowEventLog> _subscriptions = [];
    private Window? _galleryWindow;

    public MultiWindowPage(SamplePageServices services)
        : base(services)
    {
        ArgumentNullException.ThrowIfNull(services);
        InitializeComponent();
        _windows = services.Windows;

        ItemPicker.ItemsSource = ItemIds.Select(services.Catalog.Find).OfType<GalleryItem>().ToList();
        ItemPicker.SelectedIndexChanged += (_, _) => ItemRouteSubstitution.Value = SelectedItem is { } item ? $"//main/controls/{item.Id}" : null;
        ItemPicker.SelectedIndex = 0;

        var canOpen = _windows.CanOpenWindows;
        UnsupportedNote.IsVisible = !canOpen;
        ItemButton.IsEnabled = canOpen;
        CustomButton.IsEnabled = canOpen;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private GalleryItem? SelectedItem => ItemPicker.SelectedItem as GalleryItem;

    private void OnLoaded(object? sender, EventArgs e)
    {
        _galleryWindow = Window;
        if (_galleryWindow is not null)
        {
            Subscribe(_galleryWindow, "Gallery");
        }
    }

    private void OnUnloaded(object? sender, EventArgs e)
    {
        if (_galleryWindow is not null)
        {
            Unsubscribe(_galleryWindow);
            _galleryWindow = null;
        }
    }

    private void OnItemClicked(object? sender, EventArgs e)
    {
        if (SelectedItem is { } item)
        {
            _windows.OpenItem(item.Id);
            ItemExample.Output = $"OpenWindow: a new gallery window showing {item.Title}";
        }
    }

    private void OnCustomClicked(object? sender, EventArgs e)
    {
        var page = new NotesWindowPage();
        var window = new Window(page)
        {
            Title = string.IsNullOrWhiteSpace(CustomTitle.Text) ? "Notes" : CustomTitle.Text,
            MinimumWidth = 360,
            MinimumHeight = 360,
        };
        page.BindingContext = window;

        var frame = new Rect(Math.Round(CustomX.Value), Math.Round(CustomY.Value), Math.Round(CustomWidth.Value), Math.Round(CustomHeight.Value));
        window.Activated += OnFirstActivated;

        // Mac Catalyst ignores a frame that is set before the window is on screen, so apply it once it is.
        void OnFirstActivated(object? sender, EventArgs e)
        {
            window.Activated -= OnFirstActivated;
            window.X = frame.X;
            window.Y = frame.Y;
            window.Width = frame.Width;
            window.Height = frame.Height;
        }

        Subscribe(window, window.Title);
        window.Destroying += OnCustomWindowDestroying;
        _openedWindows.Add(window);
        CloseAllButton.IsEnabled = true;

        Application.Current?.OpenWindow(window);
        CustomExample.Output = $"OpenWindow: \"{window.Title}\", {window.Width:0} × {window.Height:0}";
    }

    private void OnCustomWindowDestroying(object? sender, EventArgs e)
    {
        if (sender is Window window)
        {
            window.Destroying -= OnCustomWindowDestroying;
            Unsubscribe(window);
            _openedWindows.Remove(window);
            CloseAllButton.IsEnabled = _openedWindows.Count > 0;
            CustomExample.Output = $"CloseWindow: \"{window.Title}\"";
        }
    }

    private void OnCloseAllClicked(object? sender, EventArgs e)
    {
        foreach (var window in _openedWindows.ToList())
        {
            Application.Current?.CloseWindow(window);
        }
    }

    private void OnClearLogClicked(object? sender, EventArgs e)
    {
        _log.Clear();
        LifecycleLog.Text = "No events yet";
        LifecycleExample.Output = null;
    }

    private void Subscribe(Window window, string name) =>
        _subscriptions[window] = new WindowEventLog(window, name, Log);

    private void Unsubscribe(Window window)
    {
        if (_subscriptions.Remove(window, out var subscription))
        {
            subscription.Dispose();
        }
    }

    private void Log(string line)
    {
        _log.Insert(0, $"{DateTime.Now:HH:mm:ss}  {line}");
        if (_log.Count > MaxLogLines)
        {
            _log.RemoveAt(_log.Count - 1);
        }

        LifecycleLog.Text = string.Join(Environment.NewLine, _log);
        LifecycleExample.Output = line;
    }
}
