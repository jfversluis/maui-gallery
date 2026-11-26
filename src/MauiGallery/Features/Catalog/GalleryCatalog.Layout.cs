using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Controls.Layouts;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] LayoutItems() =>
    [
        new()
        {
            Id = "grid",
            Title = "Grid",
            Description = "Arranges views in rows and columns that size to content, share space or stay fixed.",
            CategoryId = CategoryIds.Layout,
            IconGlyph = FluentIconsRegular.LayoutCellFour24,
            PageType = typeof(GridPage),
            Keywords = ["rows", "columns", "table", "cells", "span", "form", "star", "auto", "overlay", "layout"],
            NativeMappings = LayoutPanelMappings,
            RelatedIds = ["stack-layout"],
            DocsUrl = new(DocsRoot + "user-interface/layouts/grid"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.grid"),
            SourceUrl = new(SourceRoot + "Controls/Layout/Grid"),
            IsFeatured = true,
        },
        new()
        {
            Id = "stack-layout",
            Title = "StackLayout",
            Description = "Stacks views in a single vertical or horizontal line, with even spacing.",
            CategoryId = CategoryIds.Layout,
            IconGlyph = FluentIconsRegular.LayoutRowThree24,
            PageType = typeof(StackLayoutPage),
            Keywords = ["VerticalStackLayout", "HorizontalStackLayout", "stack", "row", "column", "spacing", "list", "layout"],
            NativeMappings = LayoutPanelMappings,
            RelatedIds = ["grid"],
            DocsUrl = new(DocsRoot + "user-interface/layouts/stacklayout"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.verticalstacklayout"),
            SourceUrl = new(SourceRoot + "Controls/Layout/StackLayout"),
        },
    ];

    // Every layout is rendered by the same platform container; the layout manager places the children.
    private static NativeMapping[] LayoutPanelMappings =>
    [
        new(GalleryPlatforms.iOS, "LayoutView (UIView)"),
        new(GalleryPlatforms.Android, "LayoutViewGroup"),
        new(GalleryPlatforms.MacCatalyst, "LayoutView (UIView)"),
        new(GalleryPlatforms.Windows, "LayoutPanel"),
    ];

    private static NativeMapping[] ContentPanelMappings =>
    [
        new(GalleryPlatforms.iOS, "ContentView (UIView)"),
        new(GalleryPlatforms.Android, "ContentViewGroup"),
        new(GalleryPlatforms.MacCatalyst, "ContentView (UIView)"),
        new(GalleryPlatforms.Windows, "ContentPanel"),
    ];
}
