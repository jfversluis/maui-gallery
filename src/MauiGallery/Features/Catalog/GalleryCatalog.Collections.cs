using IconFont.Maui.FluentIcons;
using MauiGallery.Core.Catalog;
using MauiGallery.Features.Controls.Collections;

namespace MauiGallery.Features.Catalog;

public sealed partial class GalleryCatalog
{
    private static GalleryItem[] CollectionsItems() =>
    [
        new()
        {
            Id = "collection-view",
            Title = "CollectionView",
            Description = "Shows a scrolling list or grid of items with selection, grouping and incremental loading.",
            CategoryId = CategoryIds.Collections,
            IconGlyph = FluentIconsRegular.AppsList24,
            PageType = typeof(CollectionViewPage),
            Keywords = ["list", "grid", "listview", "tableview", "items", "virtualization", "selection", "group", "header", "footer", "empty", "infinite scroll", "load more", "RemainingItemsThreshold"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UICollectionView"),
                new(GalleryPlatforms.Android, "RecyclerView"),
                new(GalleryPlatforms.MacCatalyst, "UICollectionView"),
                new(GalleryPlatforms.Windows, "ListView / GridView"),
            ],
            RelatedIds = ["carousel-view", "bindable-layout", "refresh-view", "swipe-view", "search-bar"],
            DocsUrl = new(DocsRoot + "user-interface/controls/collectionview/"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.collectionview"),
            SourceUrl = new(SourceRoot + "Controls/Collections/CollectionView"),
            IsFeatured = true,
            IsUpdated = true,
        },
        new()
        {
            Id = "carousel-view",
            Title = "CarouselView",
            Description = "Shows items one at a time in a scrollable, snapping carousel.",
            CategoryId = CategoryIds.Collections,
            IconGlyph = FluentIconsRegular.SlideMultiple24,
            PageType = typeof(CarouselViewPage),
            Keywords = ["carousel", "slider", "pager", "swipe", "slides", "onboarding", "gallery", "Loop", "PeekAreaInsets", "position"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "UICollectionView"),
                new(GalleryPlatforms.Android, "RecyclerView"),
                new(GalleryPlatforms.MacCatalyst, "UICollectionView"),
                new(GalleryPlatforms.Windows, "ListView"),
            ],
            RelatedIds = ["indicator-view", "collection-view"],
            DocsUrl = new(DocsRoot + "user-interface/controls/carouselview/"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.carouselview"),
            SourceUrl = new(SourceRoot + "Controls/Collections/CarouselView"),
            IsUpdated = true,
        },
        new()
        {
            Id = "indicator-view",
            Title = "IndicatorView",
            Description = "Shows dots for the items in a carousel or the steps of a flow, and which one is current.",
            CategoryId = CategoryIds.Collections,
            IconGlyph = FluentIconsRegular.MoreHorizontal24,
            PageType = typeof(IndicatorViewPage),
            Keywords = ["dots", "page control", "pager", "indicator", "steps", "carousel", "IndicatorTemplate", "MaximumVisible"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "MauiPageControl (UIPageControl)"),
                new(GalleryPlatforms.Android, "MauiPageControl (LinearLayout)"),
                new(GalleryPlatforms.MacCatalyst, "MauiPageControl (UIPageControl)"),
                new(GalleryPlatforms.Windows, "MauiPageControl (ItemsControl)"),
            ],
            RelatedIds = ["carousel-view"],
            DocsUrl = new(DocsRoot + "user-interface/controls/indicatorview"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.indicatorview"),
            SourceUrl = new(SourceRoot + "Controls/Collections/IndicatorView"),
        },
        new()
        {
            Id = "bindable-layout",
            Title = "BindableLayout",
            Description = "Fills any layout with views generated from a small collection of items.",
            CategoryId = CategoryIds.Collections,
            IconGlyph = FluentIconsRegular.TextBulletList24,
            PageType = typeof(BindableLayoutPage),
            Keywords = ["ItemsSource", "ItemTemplate", "repeater", "FlexLayout", "StackLayout", "tags", "chips", "EmptyView", "DataTemplateSelector", "template selector"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "LayoutView (UIView)"),
                new(GalleryPlatforms.Android, "LayoutViewGroup"),
                new(GalleryPlatforms.MacCatalyst, "LayoutView (UIView)"),
                new(GalleryPlatforms.Windows, "LayoutPanel"),
            ],
            RelatedIds = ["collection-view", "flex-layout", "stack-layout"],
            DocsUrl = new(DocsRoot + "user-interface/layouts/bindablelayout"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.bindablelayout"),
            SourceUrl = new(SourceRoot + "Controls/Collections/BindableLayout"),
        },
        new()
        {
            Id = "swipe-view",
            Title = "SwipeView",
            Description = "Reveals actions such as archive or delete when people swipe an item.",
            CategoryId = CategoryIds.Collections,
            IconGlyph = FluentIconsRegular.SwipeRight24,
            PageType = typeof(SwipeViewPage),
            Keywords = ["swipe", "context actions", "archive", "delete", "SwipeItem", "SwipeItemView", "reveal", "execute", "row actions"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "MauiSwipeView (UIView)"),
                new(GalleryPlatforms.Android, "MauiSwipeView (ViewGroup)"),
                new(GalleryPlatforms.MacCatalyst, "MauiSwipeView (UIView)"),
                new(GalleryPlatforms.Windows, "SwipeControl"),
            ],
            RelatedIds = ["collection-view", "bindable-layout"],
            DocsUrl = new(DocsRoot + "user-interface/controls/swipeview"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.swipeview"),
            SourceUrl = new(SourceRoot + "Controls/Collections/SwipeView"),
        },
        new()
        {
            Id = "refresh-view",
            Title = "RefreshView",
            Description = "Adds pull to refresh to a list or any other scrollable content.",
            CategoryId = CategoryIds.Collections,
            IconGlyph = FluentIconsRegular.ArrowSync24,
            PageType = typeof(RefreshViewPage),
            Keywords = ["pull to refresh", "refresh", "reload", "sync", "spinner", "IsRefreshing", "RefreshColor"],
            NativeMappings =
            [
                new(GalleryPlatforms.iOS, "MauiRefreshView (UIRefreshControl)"),
                new(GalleryPlatforms.Android, "SwipeRefreshLayout"),
                new(GalleryPlatforms.MacCatalyst, "MauiRefreshView (UIRefreshControl)"),
                new(GalleryPlatforms.Windows, "RefreshContainer"),
            ],
            RelatedIds = ["collection-view", "activity-indicator"],
            DocsUrl = new(DocsRoot + "user-interface/controls/refreshview"),
            ApiUrl = new(ApiRoot + "microsoft.maui.controls.refreshview"),
            SourceUrl = new(SourceRoot + "Controls/Collections/RefreshView"),
        },
    ];
}
