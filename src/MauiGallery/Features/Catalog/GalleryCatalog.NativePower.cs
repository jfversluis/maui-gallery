using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.NativePower;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] NativePowerItems() =>
    [
        new()
        {
            Id = "handler-customization",
            Title = "Handler customization",
            Description = "Reaches into the native control behind any .NET MAUI view with AppendToMapping, ModifyMapping and UpdateValue.",
            CategoryId = CategoryIds.NativePower,
            IconGlyph = FluentIconsRegular.WrenchScrewdriver24,
            PageType = typeof(HandlerCustomizationPage),
            Keywords = ["handler", "mapper", "AppendToMapping", "PrependToMapping", "ModifyMapping", "UpdateValue", "PlatformView", "custom renderer", "renderer", "native", "border", "underline", "tint", "caret", "haptic", "EntryHandler", "SliderHandler"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UITextField, UISlider"),
                new(GalleryPlatforms.Android, "AppCompatEditText, SeekBar"),
                new(GalleryPlatforms.MacCatalyst, "UITextField, UISlider"),
                new(GalleryPlatforms.Windows, "TextBox, Slider"),
            ],
            RelatedIds = ["native-views", "native-view-inspector", "platform-specifics", "entry", "slider", "switch"],
            DocsUrl = new(DocsRoot + "user-interface/handlers/customize"),
            ApiUrl = new(ApiRoot + "microsoft.maui.propertymapperextensions"),
            SourceUrl = new(SourceRoot + "NativePower/HandlerCustomization"),
            IsFeatured = true,
        },
        new()
        {
            Id = "platform-specifics",
            Title = "Platform-specifics",
            Description = "Turns on features that only one platform has, with attached properties the other platforms ignore.",
            CategoryId = CategoryIds.NativePower,
            IconGlyph = FluentIconsRegular.PhoneLaptop24,
            PageType = typeof(PlatformSpecificsPage),
            Keywords = ["On<iOS>", "On<Android>", "On<Windows>", "iOSSpecific", "AndroidSpecific", "WindowsSpecific", "AdjustsFontSizeToFitWidth", "CursorColor", "UpdateOnTap", "RippleColor", "ImeOptions", "NoPersonalizedLearning", "incognito", "AccessKey", "DetectReadingOrderFromContent", "WindowSoftInputModeAdjust"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UITextField, UISlider"),
                new(GalleryPlatforms.Android, "RippleDrawable, EditorInfo.imeOptions"),
                new(GalleryPlatforms.Windows, "UIElement.AccessKey, TextReadingOrder"),
            ],
            RelatedIds = ["handler-customization", "platform-code", "entry", "button", "slider"],
            DocsUrl = new(DocsRoot + "platform-integration/customize-ui-appearance"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.platformconfiguration"),
            SourceUrl = new(SourceRoot + "NativePower/PlatformSpecifics"),
        },
        new()
        {
            Id = "native-views",
            Title = "Native views",
            Description = "Brings any native control into .NET MAUI with a handler of your own: properties, commands and events.",
            CategoryId = CategoryIds.NativePower,
            IconGlyph = FluentIconsRegular.PuzzlePiece24,
            PageType = typeof(NativeViewsPage),
            Keywords = ["handler", "custom handler", "ViewHandler", "CreatePlatformView", "ConnectHandler", "DisconnectHandler", "PropertyMapper", "CommandMapper", "ConfigureMauiHandlers", "AddHandler", "calendar", "UIDatePicker", "CalendarView", "UIColorWell", "Chip", "RatingControl", "ToPlatform", "native embedding"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIDatePicker (inline), UIColorWell"),
                new(GalleryPlatforms.Android, "CalendarView, Material Chip"),
                new(GalleryPlatforms.MacCatalyst, "UIDatePicker (inline), UIColorWell"),
                new(GalleryPlatforms.Windows, "CalendarView, RatingControl"),
            ],
            RelatedIds = ["handler-customization", "native-view-inspector", "platform-code", "content-view", "custom-layout", "date-picker"],
            DocsUrl = new(DocsRoot + "user-interface/handlers/create"),
            ApiUrl = new(ApiRoot + "microsoft.maui.handlers.viewhandler-2"),
            SourceUrl = new(SourceRoot + "NativePower/NativeViews"),
            IsFeatured = true,
        },
        new()
        {
            Id = "platform-code",
            Title = "Platform code",
            Description = "Calls native APIs from shared code with runtime checks, #if blocks and partial classes per platform.",
            CategoryId = CategoryIds.NativePower,
            IconGlyph = FluentIconsRegular.CodeBlock24,
            PageType = typeof(PlatformCodePage),
            Keywords = ["#if", "conditional compilation", "ANDROID", "IOS", "MACCATALYST", "WINDOWS", "partial class", "partial method", "Platforms folder", "multi-targeting", "DeviceInfo", "DevicePlatform", "OnPlatform", "OnIdiom", "IsAndroidVersionAtLeast", "accent color", "thermal state", "dependency injection"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "NSProcessInfo, UIColor.Tint"),
                new(GalleryPlatforms.Android, "Build, PowerManager, system_accent1_500"),
                new(GalleryPlatforms.MacCatalyst, "NSProcessInfo, UIColor.Tint"),
                new(GalleryPlatforms.Windows, "AnalyticsInfo, UISettings"),
            ],
            RelatedIds = ["platform-specifics", "native-views", "handler-customization"],
            DocsUrl = new(DocsRoot + "platform-integration/invoke-platform-code"),
            ApiUrl = new(ApiRoot + "microsoft.maui.devices.deviceinfo"),
            SourceUrl = new(SourceRoot + "NativePower/PlatformCode"),
        },
        new()
        {
            Id = "native-view-inspector",
            Title = "Native view inspector",
            Description = "Shows the native control behind a .NET MAUI view, its base classes and the wrapper a Shadow adds.",
            CategoryId = CategoryIds.NativePower,
            IconGlyph = FluentIconsRegular.CubeTree24,
            PageType = typeof(NativeViewInspectorPage),
            Keywords = ["Handler", "PlatformView", "ContainerView", "WrapperView", "ToPlatform", "native type", "type hierarchy", "BaseType", "inspect", "debug", "IViewHandler"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIView subclasses, WrapperView"),
                new(GalleryPlatforms.Android, "Android.Views.View subclasses, WrapperView"),
                new(GalleryPlatforms.MacCatalyst, "UIView subclasses, WrapperView"),
                new(GalleryPlatforms.Windows, "UIElement subclasses, WrapperView"),
            ],
            RelatedIds = ["native-views", "handler-customization", "shadows"],
            DocsUrl = new(DocsRoot + "user-interface/handlers/"),
            ApiUrl = new(ApiRoot + "microsoft.maui.iviewhandler"),
            SourceUrl = new(SourceRoot + "NativePower/NativeViewInspector"),
        },
    ];
}
