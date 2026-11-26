using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Controls.BasicInput;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] BasicInputItems() =>
    [
        new()
        {
            Id = "button",
            Title = "Button",
            Description = "Responds to a tap or click to run an action.",
            CategoryId = CategoryIds.BasicInput,
            IconGlyph = FluentIconsRegular.CursorClick24,
            PageType = typeof(ButtonPage),
            Keywords = ["click", "tap", "command", "action", "press"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIButton"),
                new(GalleryPlatforms.Android, "MaterialButton"),
                new(GalleryPlatforms.MacCatalyst, "UIButton"),
                new(GalleryPlatforms.Windows, "Button"),
            ],
            RelatedIds = ["image-button", "tooltip"],
            DocsUrl = new(DocsRoot + "user-interface/controls/button"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.button"),
            SourceUrl = new(SourceRoot + "Controls/BasicInput/Button"),
            IsFeatured = true,
        },
        new()
        {
            Id = "image-button",
            Title = "ImageButton",
            Description = "A button that shows an image instead of text, for compact icon actions.",
            CategoryId = CategoryIds.BasicInput,
            IconGlyph = FluentIconsRegular.Image24,
            PageType = typeof(ImageButtonPage),
            Keywords = ["icon", "image", "button", "tap", "click", "command", "toolbar"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIButton"),
                new(GalleryPlatforms.Android, "ShapeableImageView"),
                new(GalleryPlatforms.MacCatalyst, "UIButton"),
                new(GalleryPlatforms.Windows, "Button"),
            ],
            RelatedIds = ["button", "tooltip"],
            DocsUrl = new(DocsRoot + "user-interface/controls/imagebutton"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.imagebutton"),
            SourceUrl = new(SourceRoot + "Controls/BasicInput/ImageButton"),
        },
        new()
        {
            Id = "checkbox",
            Title = "CheckBox",
            Description = "Lets the user check or uncheck an option, on its own or in a list.",
            CategoryId = CategoryIds.BasicInput,
            IconGlyph = FluentIconsRegular.CheckboxChecked24,
            PageType = typeof(CheckBoxPage),
            Keywords = ["check", "tick", "toggle", "option", "agree", "list", "boolean"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "MauiCheckBox (UIButton)"),
                new(GalleryPlatforms.Android, "AppCompatCheckBox"),
                new(GalleryPlatforms.MacCatalyst, "MauiCheckBox (UIButton)"),
                new(GalleryPlatforms.Windows, "CheckBox"),
            ],
            RelatedIds = ["switch", "radio-button"],
            DocsUrl = new(DocsRoot + "user-interface/controls/checkbox"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.checkbox"),
            SourceUrl = new(SourceRoot + "Controls/BasicInput/CheckBox"),
        },
        new()
        {
            Id = "radio-button",
            Title = "RadioButton",
            Description = "Lets the user pick exactly one option from a group of choices.",
            CategoryId = CategoryIds.BasicInput,
            IconGlyph = FluentIconsRegular.RadioButton24,
            PageType = typeof(RadioButtonPage),
            Keywords = ["radio", "option", "choice", "group", "select", "single", "ControlTemplate"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "Templated ContentView"),
                new(GalleryPlatforms.Android, "AppCompatRadioButton"),
                new(GalleryPlatforms.MacCatalyst, "Templated ContentView"),
                new(GalleryPlatforms.Windows, "RadioButton"),
            ],
            RelatedIds = ["checkbox", "switch"],
            DocsUrl = new(DocsRoot + "user-interface/controls/radiobutton"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.radiobutton"),
            SourceUrl = new(SourceRoot + "Controls/BasicInput/RadioButton"),
            IsFeatured = true,
        },
        new()
        {
            Id = "switch",
            Title = "Switch",
            Description = "Turns a setting on or off, taking effect immediately.",
            CategoryId = CategoryIds.BasicInput,
            IconGlyph = FluentIconsRegular.ToggleRight24,
            PageType = typeof(SwitchPage),
            Keywords = ["toggle", "on", "off", "setting", "boolean", "OffColor"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UISwitch"),
                new(GalleryPlatforms.Android, "SwitchCompat"),
                new(GalleryPlatforms.MacCatalyst, "UISwitch"),
                new(GalleryPlatforms.Windows, "ToggleSwitch"),
            ],
            RelatedIds = ["checkbox", "radio-button"],
            DocsUrl = new(DocsRoot + "user-interface/controls/switch"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.switch"),
            SourceUrl = new(SourceRoot + "Controls/BasicInput/Switch"),
            IsUpdated = true,
        },
        new()
        {
            Id = "slider",
            Title = "Slider",
            Description = "Picks a value from a continuous range by dragging a thumb.",
            CategoryId = CategoryIds.BasicInput,
            IconGlyph = FluentIconsRegular.Options24,
            PageType = typeof(SliderPage),
            Keywords = ["range", "value", "drag", "volume", "seek", "track", "thumb"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UISlider"),
                new(GalleryPlatforms.Android, "SeekBar"),
                new(GalleryPlatforms.MacCatalyst, "UISlider"),
                new(GalleryPlatforms.Windows, "Slider"),
            ],
            RelatedIds = ["stepper", "progress-bar"],
            DocsUrl = new(DocsRoot + "user-interface/controls/slider"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.slider"),
            SourceUrl = new(SourceRoot + "Controls/BasicInput/Slider"),
        },
        new()
        {
            Id = "stepper",
            Title = "Stepper",
            Description = "Increases or decreases a number in fixed steps.",
            CategoryId = CategoryIds.BasicInput,
            IconGlyph = FluentIconsRegular.AddSubtractCircle24,
            PageType = typeof(StepperPage),
            Keywords = ["increment", "decrement", "plus", "minus", "number", "quantity", "counter"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIStepper"),
                new(GalleryPlatforms.Android, "MauiStepper"),
                new(GalleryPlatforms.MacCatalyst, "UIStepper"),
                new(GalleryPlatforms.Windows, "MauiStepper"),
            ],
            RelatedIds = ["slider"],
            DocsUrl = new(DocsRoot + "user-interface/controls/stepper"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.stepper"),
            SourceUrl = new(SourceRoot + "Controls/BasicInput/Stepper"),
        },
    ];
}
