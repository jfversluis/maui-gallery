using System.Collections.Specialized;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class ConnectivityPage : SamplePage
{
    private readonly ConnectivityViewModel _connectivity;

    public ConnectivityPage(SamplePageServices services, ConnectivityViewModel connectivity)
        : base(services)
    {
        InitializeComponent();

        _connectivity = connectivity;
        StatusSample.BindingContext = connectivity;
        LoadSample.BindingContext = connectivity;
        ChangesSample.BindingContext = connectivity;
        LoadExample.NativeTarget = LoadButton;

        connectivity.Changes.CollectionChanged += OnChangesChanged;
        connectivity.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(ConnectivityViewModel.Headlines))
            {
                LoadExample.Output = connectivity.IsOnline ? $"NetworkAccess.Internet: {connectivity.Headlines}" : $"NetworkAccess.{connectivity.Access}: showed cached headlines";
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _connectivity.Start();
    }

    protected override void OnDisappearing()
    {
        _connectivity.Stop();
        base.OnDisappearing();
    }

    private void OnChangesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add)
        {
            ChangesExample.Output = $"ConnectivityChanged: {_connectivity.Access}";
        }
    }
}
