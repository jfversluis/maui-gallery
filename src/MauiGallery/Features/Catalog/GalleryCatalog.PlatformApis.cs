using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.PlatformApis;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] PlatformApisItems() =>
    [
        new()
        {
            Id = "permissions",
            Title = "Permissions",
            Description = "Checks and requests runtime permissions, explains why first, and points to Settings after a no.",
            CategoryId = CategoryIds.PlatformApis,
            IconGlyph = FluentIconsRegular.LockShield24,
            PageType = typeof(PermissionsPage),
            Keywords = ["Permissions", "CheckStatusAsync", "RequestAsync", "ShouldShowRationale", "PermissionStatus", "BasePermission", "BasePlatformPermission", "custom permission", "AppInfo.ShowSettingsUI", "settings", "privacy", "location", "camera", "microphone", "Info.plist", "AndroidManifest", "usage description", "runtime permission", "denied"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "CLLocationManager, AVCaptureDevice authorization"),
                new(GalleryPlatforms.Android, "ActivityCompat.RequestPermissions"),
                new(GalleryPlatforms.MacCatalyst, "CLLocationManager, AVCaptureDevice authorization"),
                new(GalleryPlatforms.Windows, "Package capabilities"),
            ],
            RelatedIds = ["geolocation", "media-picker", "flashlight", "alerts"],
            DocsUrl = new(DocsRoot + "platform-integration/appmodel/permissions"),
            ApiUrl = new(ApiRoot + "microsoft.maui.applicationmodel.permissions"),
            SourceUrl = new(SourceRoot + "PlatformApis/Permissions"),
            IsFeatured = true,
        },
        new()
        {
            Id = "geolocation",
            Title = "Geolocation",
            Description = "Finds where the device is: the last known location, a fresh fix, live updates and distances.",
            CategoryId = CategoryIds.PlatformApis,
            IconGlyph = FluentIconsRegular.Location24,
            PageType = typeof(GeolocationPage),
            Keywords = ["Geolocation", "location", "GPS", "GetLastKnownLocationAsync", "GetLocationAsync", "GeolocationRequest", "GeolocationAccuracy", "StartListeningForegroundAsync", "LocationChanged", "StopListeningForeground", "CalculateDistance", "DistanceUnits", "IsFromMockProvider", "latitude", "longitude", "coordinates", "CLLocationManager", "LocationManager"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "CLLocationManager"),
                new(GalleryPlatforms.Android, "LocationManager"),
                new(GalleryPlatforms.MacCatalyst, "CLLocationManager"),
                new(GalleryPlatforms.Windows, "Geolocator"),
            ],
            RelatedIds = ["geocoding", "permissions", "sensors"],
            DocsUrl = new(DocsRoot + "platform-integration/device/geolocation"),
            ApiUrl = new(ApiRoot + "microsoft.maui.devices.sensors.geolocation"),
            SourceUrl = new(SourceRoot + "PlatformApis/Geolocation"),
        },
        new()
        {
            Id = "geocoding",
            Title = "Geocoding",
            Description = "Turns an address into coordinates, and coordinates into an address, with the platform's geocoder.",
            CategoryId = CategoryIds.PlatformApis,
            IconGlyph = FluentIconsRegular.GlobeLocation24,
            PageType = typeof(GeocodingPage),
            Keywords = ["Geocoding", "GetLocationsAsync", "GetPlacemarksAsync", "Placemark", "address", "reverse geocoding", "coordinates", "latitude", "longitude", "CLGeocoder", "Geocoder", "MapLocationFinder"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "CLGeocoder"),
                new(GalleryPlatforms.Android, "Geocoder"),
                new(GalleryPlatforms.MacCatalyst, "CLGeocoder"),
                new(GalleryPlatforms.Windows, "MapLocationFinder"),
            ],
            RelatedIds = ["geolocation", "permissions"],
            DocsUrl = new(DocsRoot + "platform-integration/device/geocoding"),
            ApiUrl = new(ApiRoot + "microsoft.maui.devices.sensors.geocoding"),
            SourceUrl = new(SourceRoot + "PlatformApis/Geocoding"),
        },
        new()
        {
            Id = "sensors",
            Title = "Sensors",
            Description = "Reads the accelerometer, gyroscope, magnetometer, compass, barometer and orientation sensor live.",
            CategoryId = CategoryIds.PlatformApis,
            IconGlyph = FluentIconsRegular.CompassNorthwest24,
            PageType = typeof(SensorsPage),
            Keywords = ["sensors", "Accelerometer", "ShakeDetected", "shake", "Gyroscope", "Magnetometer", "Compass", "heading", "Barometer", "pressure", "altitude", "OrientationSensor", "quaternion", "tilt", "spirit level", "SensorSpeed", "ReadingChanged", "IsSupported", "CMMotionManager", "SensorManager", "motion"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "CMMotionManager, CMAltimeter"),
                new(GalleryPlatforms.Android, "SensorManager"),
                new(GalleryPlatforms.Windows, "Windows.Devices.Sensors"),
            ],
            RelatedIds = ["geolocation", "haptics-and-vibration", "graphics-view", "transforms"],
            DocsUrl = new(DocsRoot + "platform-integration/device/sensors"),
            ApiUrl = new(ApiRoot + "microsoft.maui.devices.sensors"),
            SourceUrl = new(SourceRoot + "PlatformApis/Sensors"),
            IsFeatured = true,
        },
        new()
        {
            Id = "haptics-and-vibration",
            Title = "Haptics and vibration",
            Description = "Plays the platform's haptic taps, and runs the vibration motor for as long as you need.",
            CategoryId = CategoryIds.PlatformApis,
            IconGlyph = FluentIconsRegular.HapticStrong24,
            PageType = typeof(HapticsAndVibrationPage),
            Keywords = ["HapticFeedback", "haptics", "HapticFeedbackType", "Click", "LongPress", "Vibration", "vibrate", "Cancel", "VIBRATE", "UIImpactFeedbackGenerator", "Vibrator", "feedback", "taptic"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIImpactFeedbackGenerator, AudioServices vibrate"),
                new(GalleryPlatforms.Android, "View.PerformHapticFeedback, Vibrator"),
                new(GalleryPlatforms.Windows, "VibrationDevice"),
            ],
            RelatedIds = ["sensors", "flashlight", "slider", "handler-customization"],
            DocsUrl = new(DocsRoot + "platform-integration/device/haptic-feedback"),
            ApiUrl = new(ApiRoot + "microsoft.maui.devices.hapticfeedback"),
            SourceUrl = new(SourceRoot + "PlatformApis/HapticsAndVibration"),
        },
        new()
        {
            Id = "flashlight",
            Title = "Flashlight",
            Description = "Turns the camera's torch on and off, and flashes a signal you can cancel.",
            CategoryId = CategoryIds.PlatformApis,
            IconGlyph = FluentIconsRegular.Flashlight24,
            PageType = typeof(FlashlightPage),
            Keywords = ["Flashlight", "torch", "TurnOnAsync", "TurnOffAsync", "IsSupportedAsync", "flash", "light", "LED", "Morse", "SOS", "AVCaptureDevice", "CameraManager.SetTorchMode", "Lamp"],
            Platforms = GalleryPlatforms.Android | GalleryPlatforms.iOS | GalleryPlatforms.Windows,
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "AVCaptureDevice.TorchMode"),
                new(GalleryPlatforms.Android, "CameraManager.SetTorchMode"),
                new(GalleryPlatforms.Windows, "Lamp"),
            ],
            RelatedIds = ["haptics-and-vibration", "permissions"],
            DocsUrl = new(DocsRoot + "platform-integration/device/flashlight"),
            ApiUrl = new(ApiRoot + "microsoft.maui.devices.flashlight"),
            SourceUrl = new(SourceRoot + "PlatformApis/Flashlight"),
        },
        new()
        {
            Id = "media-picker",
            Title = "Media picker",
            Description = "Picks one or several photos, takes photos with the camera, and picks or records videos.",
            CategoryId = CategoryIds.PlatformApis,
            IconGlyph = FluentIconsRegular.ImageAdd24,
            PageType = typeof(MediaPickerPage),
            Keywords = ["MediaPicker", "PickPhotoAsync", "PickPhotosAsync", "CapturePhotoAsync", "PickVideoAsync", "PickVideosAsync", "CaptureVideoAsync", "MediaPickerOptions", "SelectionLimit", "CompressionQuality", "MaximumWidth", "IsCaptureSupported", "photo", "camera", "gallery", "video", "FileResult", "PHPickerViewController", "photo picker"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "PHPickerViewController, UIImagePickerController"),
                new(GalleryPlatforms.Android, "Photo Picker, MediaStore.ACTION_IMAGE_CAPTURE"),
                new(GalleryPlatforms.MacCatalyst, "PHPickerViewController"),
                new(GalleryPlatforms.Windows, "FileOpenPicker, CameraCaptureUI"),
            ],
            RelatedIds = ["permissions", "flashlight"],
            DocsUrl = new(DocsRoot + "platform-integration/device-media/picker"),
            ApiUrl = new(ApiRoot + "microsoft.maui.media.mediapicker"),
            SourceUrl = new(SourceRoot + "PlatformApis/MediaPicker"),
            IsNew = true,
        },
    ];
}
