namespace MauiGallery.Features.Navigation;

/// <summary>The navigation bar settings that the navigation page sample applies to its pages.</summary>
public sealed record NavigationDemoOptions(bool CustomTitleView, string? BackButtonTitle, bool HasNavigationBar, bool HasBackButton)
{
    public static NavigationDemoOptions Default { get; } = new(false, null, true, true);
}
