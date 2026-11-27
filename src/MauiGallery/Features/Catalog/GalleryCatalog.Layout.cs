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
            RelatedIds = ["stack-layout", "flex-layout", "absolute-layout"],
            DocsUrl = new(DocsRoot + "user-interface/layouts/grid"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.grid"),
            SourceUrl = new(SourceRoot + "Controls/Layout/Grid"),
            FeaturedTask = "Build a layout",
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
            RelatedIds = ["grid", "flex-layout", "scroll-view"],
            DocsUrl = new(DocsRoot + "user-interface/layouts/stacklayout"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.verticalstacklayout"),
            SourceUrl = new(SourceRoot + "Controls/Layout/StackLayout"),
        },
        new()
        {
            Id = "flex-layout",
            Title = "FlexLayout",
            Description = "Lays out views in rows or columns that wrap, grow and shrink, like CSS flexbox.",
            CategoryId = CategoryIds.Layout,
            IconGlyph = FluentIconsRegular.LayoutDynamic24,
            PageType = typeof(FlexLayoutPage),
            Keywords = ["flexbox", "wrap", "justify", "align", "grow", "shrink", "basis", "tags", "chips", "responsive", "layout"],
            NativeMappings = LayoutPanelMappings,
            RelatedIds = ["stack-layout", "grid", "custom-layout", "bindable-layout"],
            DocsUrl = new(DocsRoot + "user-interface/layouts/flexlayout"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.flexlayout"),
            SourceUrl = new(SourceRoot + "Controls/Layout/FlexLayout"),
        },
        new()
        {
            Id = "absolute-layout",
            Title = "AbsoluteLayout",
            Description = "Positions and sizes views with explicit or proportional bounds, for overlays and badges.",
            CategoryId = CategoryIds.Layout,
            IconGlyph = FluentIconsRegular.PositionToFront24,
            PageType = typeof(AbsoluteLayoutPage),
            Keywords = ["position", "bounds", "coordinates", "proportional", "overlay", "badge", "layer", "layout"],
            NativeMappings = LayoutPanelMappings,
            RelatedIds = ["grid", "custom-layout"],
            DocsUrl = new(DocsRoot + "user-interface/layouts/absolutelayout"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.absolutelayout"),
            SourceUrl = new(SourceRoot + "Controls/Layout/AbsoluteLayout"),
        },
        new()
        {
            Id = "two-pane-view",
            Title = "TwoPaneView",
            Description = "Shows two panes side by side or stacked, and splits them along the fold on Android foldables.",
            CategoryId = CategoryIds.Layout,
            IconGlyph = FluentIconsRegular.SplitVertical24,
            PageType = typeof(TwoPaneViewPage),
            Keywords = ["foldable", "fold", "hinge", "dual screen", "Surface Duo", "split view", "master detail", "list detail", "DualScreenInfo", "span", "adaptive", "layout"],
            NativeMappings = LayoutPanelMappings,
            RelatedIds = ["grid", "responsive-design", "flyout-page"],
            DocsUrl = new(DocsRoot + "user-interface/controls/twopaneview"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.foldable.twopaneview"),
            SourceUrl = new(SourceRoot + "Controls/Layout/TwoPaneView"),
        },
        new()
        {
            Id = "scroll-view",
            Title = "ScrollView",
            Description = "Scrolls content that is larger than the space available, vertically, horizontally or both.",
            CategoryId = CategoryIds.Layout,
            IconGlyph = FluentIconsRegular.ArrowAutofitHeight24,
            PageType = typeof(ScrollViewPage),
            Keywords = ["scroll", "scrolling", "pan", "overflow", "ScrollToAsync", "scroll bar", "offset"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UIScrollView"),
                new(GalleryPlatforms.Android, "MauiScrollView (NestedScrollView)"),
                new(GalleryPlatforms.MacCatalyst, "UIScrollView"),
                new(GalleryPlatforms.Windows, "ScrollViewer"),
            ],
            RelatedIds = ["stack-layout", "grid", "collection-view"],
            DocsUrl = new(DocsRoot + "user-interface/controls/scrollview"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.scrollview"),
            SourceUrl = new(SourceRoot + "Controls/Layout/ScrollView"),
        },
        new()
        {
            Id = "border",
            Title = "Border",
            Description = "Draws a stroke, background and shadow around one view, in any shape. Replaces Frame.",
            CategoryId = CategoryIds.Layout,
            IconGlyph = FluentIconsRegular.BorderOutside24,
            PageType = typeof(BorderPage),
            Keywords = ["Frame", "stroke", "outline", "corner radius", "rounded", "shadow", "dash", "gradient", "card", "shape"],
            NativeMappings = ContentPanelMappings,
            RelatedIds = ["content-view", "grid"],
            DocsUrl = new(DocsRoot + "user-interface/controls/border"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.border"),
            SourceUrl = new(SourceRoot + "Controls/Layout/Border"),
        },
        new()
        {
            Id = "content-view",
            Title = "ContentView",
            Description = "Hosts a single view, and is the starting point for your own reusable controls.",
            CategoryId = CategoryIds.Layout,
            IconGlyph = FluentIconsRegular.ContentView24,
            PageType = typeof(ContentViewPage),
            Keywords = ["custom control", "reusable", "user control", "component", "BindableProperty", "ControlTemplate", "ContentPresenter"],
            NativeMappings = ContentPanelMappings,
            RelatedIds = ["custom-layout", "border"],
            DocsUrl = new(DocsRoot + "user-interface/controls/contentview"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.contentview"),
            SourceUrl = new(SourceRoot + "Controls/Layout/ContentView"),
        },
        new()
        {
            Id = "custom-layout",
            Title = "Custom layout",
            Description = "Write your own layout by pairing a Layout subclass with an ILayoutManager.",
            CategoryId = CategoryIds.Layout,
            IconGlyph = FluentIconsRegular.PuzzlePiece24,
            PageType = typeof(CustomLayoutPage),
            Keywords = ["ILayoutManager", "Layout", "measure", "arrange", "circular", "wrap", "uniform grid", "panel"],
            NativeMappings = LayoutPanelMappings,
            RelatedIds = ["flex-layout", "absolute-layout", "content-view"],
            DocsUrl = new(DocsRoot + "user-interface/layouts/custom"),
            ApiUrl = new(ApiRoot + "microsoft.maui.layouts.ilayoutmanager"),
            SourceUrl = new(SourceRoot + "Controls/Layout/CustomLayout"),
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
