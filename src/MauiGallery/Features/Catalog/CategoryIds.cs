namespace MauiGallery.Features.Catalog;

public static class CategoryIds
{
    public const string DesignGuidance = "design-guidance";
    public const string BasicInput = "basic-input";
    public const string Text = "text";
    public const string Pickers = "pickers";
    public const string Collections = "collections";
    public const string Layout = "layout";
    public const string StatusAndInfo = "status-and-info";
    public const string Dialogs = "dialogs";
    public const string MenusAndToolbars = "menus-and-toolbars";
    public const string Media = "media";
    public const string Graphics = "graphics";
    public const string Fundamentals = "fundamentals";
    public const string Navigation = "navigation";
    public const string DeviceAndSensors = "device-and-sensors";
    public const string SystemAndSharing = "system-and-sharing";
    public const string NativePower = "native-power";

    /// <summary>Platform APIs was split into <see cref="DeviceAndSensors"/> and <see cref="SystemAndSharing"/>; old links open the first.</summary>
    public const string LegacyPlatformApis = "platform-apis";

    public static string Resolve(string id) => id == LegacyPlatformApis ? DeviceAndSensors : id;
}
