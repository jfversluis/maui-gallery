using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MauiGallery.Features.PlatformApis;

/// <summary>The view model behind the Connectivity samples.</summary>
public sealed partial class ConnectivityViewModel(IConnectivity connectivity) : ObservableObject
{
    private const int MaxLogEntries = 5;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOnline))]
    [NotifyCanExecuteChangedFor(nameof(LoadHeadlinesCommand))]
    public partial NetworkAccess Access { get; set; } = connectivity.NetworkAccess;

    [ObservableProperty]
    public partial string Profiles { get; set; } = Describe(connectivity.ConnectionProfiles);

    [ObservableProperty]
    public partial string Headlines { get; set; } = "Tap Load to fetch headlines";

    public bool IsOnline => Access == NetworkAccess.Internet;

    /// <summary>The latest ConnectivityChanged events, newest first.</summary>
    public ObservableCollection<string> Changes { get; } = [];

    /// <summary>Starts listening for network changes. Call it when the page appears.</summary>
    public void Start()
    {
        Access = connectivity.NetworkAccess;
        Profiles = Describe(connectivity.ConnectionProfiles);
        connectivity.ConnectivityChanged += OnConnectivityChanged;
    }

    /// <summary>Stops listening. Call it when the page disappears.</summary>
    public void Stop() => connectivity.ConnectivityChanged -= OnConnectivityChanged;

    [RelayCommand]
    private async Task LoadHeadlinesAsync()
    {
        // Check first: a request without a network fails slowly, after a timeout.
        if (connectivity.NetworkAccess != NetworkAccess.Internet)
        {
            Headlines = "You're offline. Showing the headlines you saved earlier.";
            return;
        }

        Headlines = "Loading…";

        // Stands in for a web request.
        await Task.Delay(TimeSpan.FromSeconds(1));
        Headlines = $"12 new headlines at {DateTime.Now:t}";
    }

    private void OnConnectivityChanged(object? sender, ConnectivityChangedEventArgs e)
    {
        Access = e.NetworkAccess;
        Profiles = Describe(e.ConnectionProfiles);

        Changes.Insert(0, $"{DateTime.Now:T}  {Access} over {Profiles}");
        if (Changes.Count > MaxLogEntries)
        {
            Changes.RemoveAt(Changes.Count - 1);
        }
    }

    private static string Describe(IEnumerable<ConnectionProfile> profiles)
    {
        var list = profiles.ToList();
        return list.Count == 0 ? "No connection" : string.Join(", ", list);
    }
}
