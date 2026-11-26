using System.ComponentModel;
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.PlatformApis;

public sealed partial class PermissionsPage : SamplePage
{
    private readonly PermissionsViewModel _permissions = new();

    public PermissionsPage(SamplePageServices services, CameraAccessViewModel cameraAccess)
        : base(services)
    {
        InitializeComponent();
        CheckSample.BindingContext = _permissions;
        RequestSample.BindingContext = _permissions;
        CustomSample.BindingContext = _permissions.VideoCapture;
        FlowSample.BindingContext = cameraAccess;
        RequestExample.NativeTarget = RequestButton;
        FlowExample.NativeTarget = FlowButton;
        CustomExample.NativeTarget = CustomButton;

        RequestPermission.ItemsSource = _permissions.Items.ToList();
        RequestPermission.SelectedItem = _permissions.Selected;
        RequestPermission.SelectedIndexChanged += OnRequestPermissionChanged;
        RequestPermissionSubstitution.Value = _permissions.Selected.TypeName;

        foreach (var item in _permissions.Items)
        {
            item.PropertyChanged += OnRequestStatusChanged;
        }

        _permissions.VideoCapture.PropertyChanged += OnCustomStatusChanged;
        Appearing += OnAppearing;
        if (PermissionsViewModel.CheckingLocationPrompts)
        {
            CheckExample.Notice = "On Windows, CheckStatusAsync for location calls Geolocator.RequestAccessAsync, which can show the location prompt. This page only checks location when you tap Check again.";
        }

        cameraAccess.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(CameraAccessViewModel.Status))
            {
                FlowExample.Output = $"Camera: {cameraAccess.Status}";
            }
        };
    }

    // Statuses change outside the app, in Settings, so read them again every time the page appears.
    private async void OnAppearing(object? sender, EventArgs e) => await CheckAllAsync(userInitiated: false);

    private async void OnCheckAgainClicked(object? sender, EventArgs e) => await CheckAllAsync(userInitiated: true);

    private async Task CheckAllAsync(bool userInitiated)
    {
        await _permissions.CheckAllAsync(userInitiated);
        CheckExample.Output = $"Checked at {DateTime.Now:T}";
    }

    private void OnRequestPermissionChanged(object? sender, EventArgs e)
    {
        if (RequestPermission.SelectedItem is PermissionItem item)
        {
            _permissions.Selected = item;
            RequestPermissionSubstitution.Value = item.TypeName;
        }
    }

    private void OnRequestStatusChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender == _permissions.Selected && e.PropertyName == nameof(PermissionItem.Message) && _permissions.Selected.Message is not null)
        {
            RequestExample.Output = $"RequestAsync<{_permissions.Selected.TypeName}>: {_permissions.Selected.Status}";
        }
    }

    private void OnCustomStatusChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PermissionItem.Status))
        {
            CustomExample.Output = $"CameraAndMicrophone: {_permissions.VideoCapture.Status}";
        }
    }
}
