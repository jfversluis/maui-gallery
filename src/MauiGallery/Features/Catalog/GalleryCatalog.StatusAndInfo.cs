using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Controls.StatusAndInfo;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] StatusAndInfoItems() =>
    [
        new()
        {
            Id = "activity-indicator",
            Title = "ActivityIndicator",
            Description = "A spinner that shows work is in progress when you can't tell how long it takes.",
            CategoryId = CategoryIds.StatusAndInfo,
            IconGlyph = FluentIconsRegular.HourglassHalf24,
            PageType = typeof(ActivityIndicatorPage),
            Keywords = ["spinner", "loading", "busy", "wait", "progress", "IsBusy", "indeterminate"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIActivityIndicatorView"),
                new(GalleryPlatforms.Android, "ProgressBar (indeterminate)"),
                new(GalleryPlatforms.MacCatalyst, "UIActivityIndicatorView"),
                new(GalleryPlatforms.Windows, "ProgressRing"),
            ],
            RelatedIds = ["progress-bar"],
            DocsUrl = new(DocsRoot + "user-interface/controls/activityindicator"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.activityindicator"),
            SourceUrl = new(SourceRoot + "Controls/StatusAndInfo/ActivityIndicator"),
        },
        new()
        {
            Id = "progress-bar",
            Title = "ProgressBar",
            Description = "Shows how much of a task is complete, and can animate to a new value.",
            CategoryId = CategoryIds.StatusAndInfo,
            IconGlyph = FluentIconsRegular.DataBarHorizontal24,
            PageType = typeof(ProgressBarPage),
            Keywords = ["progress", "loading", "download", "upload", "percent", "ProgressTo", "IProgress", "determinate"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIProgressView"),
                new(GalleryPlatforms.Android, "ProgressBar (horizontal)"),
                new(GalleryPlatforms.MacCatalyst, "UIProgressView"),
                new(GalleryPlatforms.Windows, "ProgressBar"),
            ],
            RelatedIds = ["activity-indicator", "slider"],
            DocsUrl = new(DocsRoot + "user-interface/controls/progressbar"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.progressbar"),
            SourceUrl = new(SourceRoot + "Controls/StatusAndInfo/ProgressBar"),
        },
        new()
        {
            Id = "tooltip",
            Title = "ToolTip",
            Description = "Shows a short hint when the pointer hovers over a view, or on long-press on Android.",
            CategoryId = CategoryIds.StatusAndInfo,
            IconGlyph = FluentIconsRegular.TooltipQuote24,
            PageType = typeof(ToolTipPage),
            Keywords = ["tooltip", "hint", "hover", "pointer", "help", "ToolTipProperties"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIToolTipInteraction"),
                new(GalleryPlatforms.Android, "TooltipCompat"),
                new(GalleryPlatforms.MacCatalyst, "UIToolTipInteraction"),
                new(GalleryPlatforms.Windows, "ToolTipService"),
            ],
            RelatedIds = ["button", "image-button"],
            DocsUrl = new(DocsRoot + "user-interface/tooltips"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.tooltipproperties"),
            SourceUrl = new(SourceRoot + "Controls/StatusAndInfo/ToolTip"),
        },
    ];
}
