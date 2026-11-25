namespace MauiGallery.Services;

public sealed class WindowService(GalleryWindowFactory factory, IDeviceInfo deviceInfo) : IWindowService
{
    public bool CanOpenWindows =>
        deviceInfo.Idiom == DeviceIdiom.Desktop ||
        (deviceInfo.Platform == DevicePlatform.iOS && deviceInfo.Idiom == DeviceIdiom.Tablet);

    public void OpenNew() => Open(factory.Create());

    public void OpenItem(string itemId) => Open(factory.Create(itemId));

    private void Open(Window window)
    {
        if (CanOpenWindows)
        {
            Application.Current?.OpenWindow(window);
        }
    }
}
