using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.PlatformApis;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] PlatformApisCoreItems() =>
    [
        new()
        {
            Id = "app-and-device-info",
            Title = "App and device info",
            Description = "Reads the app's version, the device model and platform, and the screen's size, density and orientation.",
            CategoryId = CategoryIds.DeviceAndSensors,
            IconGlyph = FluentIconsRegular.Phone24,
            PageType = typeof(AppAndDeviceInfoPage),
            Keywords = ["AppInfo", "DeviceInfo", "DeviceDisplay", "IAppInfo", "IDeviceInfo", "IDeviceDisplay", "version", "build", "package name", "bundle id", "model", "manufacturer", "idiom", "platform", "simulator", "emulator", "DeviceType", "screen size", "density", "orientation", "rotation", "refresh rate", "KeepScreenOn", "MainDisplayInfo", "ShowSettingsUI", "app settings"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "NSBundle, UIDevice, UIScreen"),
                new(GalleryPlatforms.Android, "PackageManager, Build, Display"),
                new(GalleryPlatforms.MacCatalyst, "NSBundle, UIDevice, UIScreen"),
                new(GalleryPlatforms.Windows, "Package, EasClientDeviceInformation"),
            ],
            RelatedIds = ["battery", "version-tracking", "preferences-and-secure-storage", "platform-code"],
            DocsUrl = new(DocsRoot + "platform-integration/device/information"),
            ApiUrl = new(ApiRoot + "microsoft.maui.devices.deviceinfo"),
            SourceUrl = new(SourceRoot + "PlatformApis/AppAndDeviceInfo"),
        },
        new()
        {
            Id = "battery",
            Title = "Battery",
            Description = "Reads the charge level, charging state, power source and energy saver, and reacts as they change.",
            CategoryId = CategoryIds.DeviceAndSensors,
            IconGlyph = FluentIconsRegular.BatteryCharge24,
            PageType = typeof(BatteryPage),
            Keywords = ["Battery", "IBattery", "ChargeLevel", "BatteryState", "charging", "PowerSource", "AC", "USB", "wireless", "EnergySaverStatus", "energy saver", "Low Power Mode", "battery saver", "BatteryInfoChanged", "EnergySaverStatusChanged", "BATTERY_STATS"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIDevice, NSProcessInfo"),
                new(GalleryPlatforms.Android, "BatteryManager, PowerManager"),
                new(GalleryPlatforms.MacCatalyst, "UIDevice, NSProcessInfo"),
                new(GalleryPlatforms.Windows, "Battery, PowerManager"),
            ],
            RelatedIds = ["app-and-device-info", "connectivity"],
            DocsUrl = new(DocsRoot + "platform-integration/device/battery"),
            ApiUrl = new(ApiRoot + "microsoft.maui.devices.ibattery"),
            SourceUrl = new(SourceRoot + "PlatformApis/Battery"),
        },
        new()
        {
            Id = "connectivity",
            Title = "Connectivity",
            Description = "Checks whether the device is online and over which connection, and hears when that changes.",
            CategoryId = CategoryIds.DeviceAndSensors,
            IconGlyph = FluentIconsRegular.Wifi124,
            PageType = typeof(ConnectivityPage),
            Keywords = ["Connectivity", "IConnectivity", "NetworkAccess", "internet", "offline", "online", "ConnectionProfiles", "WiFi", "cellular", "ethernet", "ConnectivityChanged", "airplane mode", "captive portal", "ACCESS_NETWORK_STATE"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "NWPathMonitor"),
                new(GalleryPlatforms.Android, "ConnectivityManager"),
                new(GalleryPlatforms.MacCatalyst, "NWPathMonitor"),
                new(GalleryPlatforms.Windows, "NetworkInformation"),
            ],
            RelatedIds = ["battery", "launcher-and-browser"],
            DocsUrl = new(DocsRoot + "platform-integration/communication/networking"),
            ApiUrl = new(ApiRoot + "microsoft.maui.networking.iconnectivity"),
            SourceUrl = new(SourceRoot + "PlatformApis/Connectivity"),
        },
        new()
        {
            Id = "clipboard",
            Title = "Clipboard",
            Description = "Copies text to the system clipboard, pastes it back and notices when the clipboard changes.",
            CategoryId = CategoryIds.SystemAndSharing,
            IconGlyph = FluentIconsRegular.ClipboardPaste24,
            PageType = typeof(ClipboardPage),
            Keywords = ["Clipboard", "IClipboard", "copy", "paste", "SetTextAsync", "GetTextAsync", "HasText", "ClipboardContentChanged", "pasteboard", "UIPasteboard", "ClipboardManager"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIPasteboard"),
                new(GalleryPlatforms.Android, "ClipboardManager"),
                new(GalleryPlatforms.MacCatalyst, "UIPasteboard"),
                new(GalleryPlatforms.Windows, "DataTransfer.Clipboard"),
            ],
            RelatedIds = ["share", "entry"],
            DocsUrl = new(DocsRoot + "platform-integration/data/clipboard"),
            ApiUrl = new(ApiRoot + "microsoft.maui.applicationmodel.datatransfer.iclipboard"),
            SourceUrl = new(SourceRoot + "PlatformApis/Clipboard"),
        },
        new()
        {
            Id = "share",
            Title = "Share",
            Description = "Opens the system share sheet to send text, a link or a file to any app.",
            CategoryId = CategoryIds.SystemAndSharing,
            IconGlyph = FluentIconsRegular.Share24,
            PageType = typeof(SharePage),
            Keywords = ["Share", "IShare", "share sheet", "ShareTextRequest", "ShareFileRequest", "ShareMultipleFilesRequest", "ShareFile", "PresentationSourceBounds", "popover", "UIActivityViewController", "Intent.ActionSend", "DataTransferManager", "AirDrop", "CacheDirectory"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIActivityViewController"),
                new(GalleryPlatforms.Android, "Intent.ActionSend (chooser)"),
                new(GalleryPlatforms.MacCatalyst, "UIActivityViewController"),
                new(GalleryPlatforms.Windows, "DataTransferManager"),
            ],
            RelatedIds = ["clipboard", "email-and-sms", "screenshot", "file-picker"],
            DocsUrl = new(DocsRoot + "platform-integration/data/share"),
            ApiUrl = new(ApiRoot + "microsoft.maui.applicationmodel.datatransfer.ishare"),
            SourceUrl = new(SourceRoot + "PlatformApis/Share"),
        },
        new()
        {
            Id = "launcher-and-browser",
            Title = "Launcher, browser and maps",
            Description = "Opens web pages in an in-app or external browser, other apps by URI, and places in the map app.",
            CategoryId = CategoryIds.SystemAndSharing,
            IconGlyph = FluentIconsRegular.Open24,
            PageType = typeof(LauncherAndBrowserPage),
            Keywords = ["Launcher", "ILauncher", "CanOpenAsync", "OpenAsync", "TryOpenAsync", "URI scheme", "custom scheme", "deep link", "mailto", "tel", "Browser", "IBrowser", "BrowserLaunchMode", "SystemPreferred", "External", "BrowserLaunchOptions", "PreferredToolbarColor", "SFSafariViewController", "Custom Tabs", "Map", "IMap", "MapLaunchOptions", "NavigationMode", "Placemark", "directions", "LSApplicationQueriesSchemes", "queries"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIApplication, SFSafariViewController"),
                new(GalleryPlatforms.Android, "Intent.ActionView, Custom Tabs"),
                new(GalleryPlatforms.MacCatalyst, "UIApplication.OpenUrl"),
                new(GalleryPlatforms.Windows, "Windows.System.Launcher"),
            ],
            RelatedIds = ["email-and-sms", "share", "deep-links"],
            DocsUrl = new(DocsRoot + "platform-integration/appmodel/launcher"),
            ApiUrl = new(ApiRoot + "microsoft.maui.applicationmodel.ilauncher"),
            SourceUrl = new(SourceRoot + "PlatformApis/LauncherAndBrowser"),
        },
        new()
        {
            Id = "email-and-sms",
            Title = "Email and SMS",
            Description = "Opens the mail or messages app with a draft filled in, ready for the person to send.",
            CategoryId = CategoryIds.SystemAndSharing,
            IconGlyph = FluentIconsRegular.MailEdit24,
            PageType = typeof(EmailAndSmsPage),
            Keywords = ["Email", "IEmail", "EmailMessage", "ComposeAsync", "IsComposeSupported", "EmailAttachment", "EmailBodyFormat", "HTML", "mail", "MFMailComposeViewController", "Sms", "ISms", "SmsMessage", "text message", "MFMessageComposeViewController", "mailto"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "MFMail/MFMessageComposeViewController"),
                new(GalleryPlatforms.Android, "Intent.ActionSendto"),
                new(GalleryPlatforms.MacCatalyst, "MFMailComposeViewController"),
                new(GalleryPlatforms.Windows, "EmailManager, ChatMessageManager"),
            ],
            RelatedIds = ["share", "launcher-and-browser"],
            DocsUrl = new(DocsRoot + "platform-integration/communication/email"),
            ApiUrl = new(ApiRoot + "microsoft.maui.applicationmodel.communication.iemail"),
            SourceUrl = new(SourceRoot + "PlatformApis/EmailAndSms"),
        },
        new()
        {
            Id = "preferences-and-secure-storage",
            Title = "Preferences and secure storage",
            Description = "Saves settings by key with Preferences, and keeps tokens and other secrets encrypted with SecureStorage.",
            CategoryId = CategoryIds.SystemAndSharing,
            IconGlyph = FluentIconsRegular.LockClosedKey24,
            PageType = typeof(PreferencesAndSecureStoragePage),
            Keywords = ["Preferences", "IPreferences", "settings", "key value", "Get", "Set", "Remove", "Clear", "sharedName", "NSUserDefaults", "SharedPreferences", "SecureStorage", "ISecureStorage", "SetAsync", "GetAsync", "Keychain", "Keystore", "DPAPI", "token", "secret", "encryption"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "NSUserDefaults, Keychain"),
                new(GalleryPlatforms.Android, "SharedPreferences, Keystore"),
                new(GalleryPlatforms.MacCatalyst, "NSUserDefaults, Keychain"),
                new(GalleryPlatforms.Windows, "LocalSettings, DPAPI"),
            ],
            RelatedIds = ["version-tracking", "file-picker", "app-and-device-info"],
            DocsUrl = new(DocsRoot + "platform-integration/storage/preferences"),
            ApiUrl = new(ApiRoot + "microsoft.maui.storage.ipreferences"),
            SourceUrl = new(SourceRoot + "PlatformApis/PreferencesAndSecureStorage"),
        },
        new()
        {
            Id = "version-tracking",
            Title = "Version tracking",
            Description = "Knows whether this is the first launch, of the app or of this version, and which versions ran before.",
            CategoryId = CategoryIds.SystemAndSharing,
            IconGlyph = FluentIconsRegular.History24,
            PageType = typeof(VersionTrackingPage),
            Keywords = ["VersionTracking", "IVersionTracking", "UseVersionTracking", "Track", "IsFirstLaunchEver", "IsFirstLaunchForCurrentVersion", "IsFirstLaunchForCurrentBuild", "PreviousVersion", "FirstInstalledVersion", "VersionHistory", "BuildHistory", "onboarding", "what's new", "upgrade", "migration"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "NSUserDefaults"),
                new(GalleryPlatforms.Android, "SharedPreferences"),
                new(GalleryPlatforms.MacCatalyst, "NSUserDefaults"),
                new(GalleryPlatforms.Windows, "ApplicationData.LocalSettings"),
            ],
            RelatedIds = ["app-and-device-info", "preferences-and-secure-storage"],
            DocsUrl = new(DocsRoot + "platform-integration/appmodel/version-tracking"),
            ApiUrl = new(ApiRoot + "microsoft.maui.applicationmodel.iversiontracking"),
            SourceUrl = new(SourceRoot + "PlatformApis/VersionTracking"),
        },
        new()
        {
            Id = "text-to-speech",
            Title = "Text-to-speech",
            Description = "Reads text aloud with the device's built-in voices, with a choice of voice, pitch, volume and rate.",
            CategoryId = CategoryIds.SystemAndSharing,
            IconGlyph = FluentIconsRegular.Speaker224,
            PageType = typeof(TextToSpeechPage),
            Keywords = ["TextToSpeech", "ITextToSpeech", "TTS", "speech", "speak", "SpeakAsync", "voice", "GetLocalesAsync", "Locale", "SpeechOptions", "pitch", "volume", "rate", "AVSpeechSynthesizer", "accessibility", "read aloud"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "AVSpeechSynthesizer"),
                new(GalleryPlatforms.Android, "TextToSpeech (android.speech.tts)"),
                new(GalleryPlatforms.MacCatalyst, "AVSpeechSynthesizer"),
                new(GalleryPlatforms.Windows, "SpeechSynthesizer"),
            ],
            RelatedIds = ["accessibility", "app-and-device-info"],
            DocsUrl = new(DocsRoot + "platform-integration/device-media/text-to-speech"),
            ApiUrl = new(ApiRoot + "microsoft.maui.media.itexttospeech"),
            SourceUrl = new(SourceRoot + "PlatformApis/TextToSpeech"),
        },
        new()
        {
            Id = "screenshot",
            Title = "Screenshot",
            Description = "Captures the app window, or a single view, as a PNG or JPEG image.",
            CategoryId = CategoryIds.SystemAndSharing,
            IconGlyph = FluentIconsRegular.Screenshot24,
            PageType = typeof(ScreenshotPage),
            Keywords = ["Screenshot", "IScreenshot", "CaptureAsync", "IsCaptureSupported", "IScreenshotResult", "OpenReadAsync", "ScreenshotFormat", "PNG", "JPEG", "capture", "render to image", "snapshot", "view.CaptureAsync"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIGraphicsImageRenderer"),
                new(GalleryPlatforms.Android, "View.Draw to a Bitmap"),
                new(GalleryPlatforms.MacCatalyst, "UIGraphicsImageRenderer"),
                new(GalleryPlatforms.Windows, "RenderTargetBitmap"),
            ],
            RelatedIds = ["share", "file-picker", "graphics-view"],
            DocsUrl = new(DocsRoot + "platform-integration/device-media/screenshot"),
            ApiUrl = new(ApiRoot + "microsoft.maui.media.iscreenshot"),
            SourceUrl = new(SourceRoot + "PlatformApis/Screenshot"),
        },
        new()
        {
            Id = "file-picker",
            Title = "File picker",
            Description = "Lets people choose one or more files, reads them, and saves and opens files the app owns.",
            CategoryId = CategoryIds.SystemAndSharing,
            IconGlyph = FluentIconsRegular.FolderOpen24,
            PageType = typeof(FilePickerPage),
            Keywords = ["FilePicker", "IFilePicker", "PickAsync", "PickMultipleAsync", "PickOptions", "FilePickerFileType", "FileResult", "OpenReadAsync", "UTI", "MIME type", "document picker", "UIDocumentPickerViewController", "FileSystem", "AppDataDirectory", "CacheDirectory", "OpenFileRequest", "ReadOnlyFile", "FileSaver", "save file"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIDocumentPickerViewController"),
                new(GalleryPlatforms.Android, "Intent.ActionOpenDocument"),
                new(GalleryPlatforms.MacCatalyst, "UIDocumentPickerViewController"),
                new(GalleryPlatforms.Windows, "FileOpenPicker"),
            ],
            RelatedIds = ["share", "preferences-and-secure-storage", "screenshot"],
            DocsUrl = new(DocsRoot + "platform-integration/storage/file-picker"),
            ApiUrl = new(ApiRoot + "microsoft.maui.storage.ifilepicker"),
            SourceUrl = new(SourceRoot + "PlatformApis/FilePicker"),
        },
    ];
}
