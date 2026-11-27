namespace MauiGallery.Core.Layout;

/// <summary>How a window shows the app's navigation.</summary>
public enum NavigationChrome
{
    /// <summary>Bottom tabs, for small touch windows.</summary>
    Tabs,

    /// <summary>A sidebar that slides in over the content.</summary>
    FlyoutSidebar,

    /// <summary>A sidebar that stays open beside the content.</summary>
    DockedSidebar,
}
