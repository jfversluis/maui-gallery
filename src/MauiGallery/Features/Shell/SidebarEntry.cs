using CommunityToolkit.Mvvm.ComponentModel;

namespace MauiGallery.Features.Shell;

/// <summary>One navigable row in the sidebar.</summary>
public sealed partial class SidebarEntry(string title, string glyph, string route, string? itemId = null) : ObservableObject
{
    public string Title { get; } = title;

    public string Glyph { get; } = glyph;

    public string Route { get; } = route;

    /// <summary>The catalog item this entry opens, if any.</summary>
    public string? ItemId { get; } = itemId;

    public string AutomationId => $"nav-{ItemId ?? Title.ToLowerInvariant().Replace(' ', '-')}";

    [ObservableProperty]
    public partial bool IsSelected { get; set; }
}
