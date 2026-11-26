using CommunityToolkit.Mvvm.ComponentModel;
using MauiGallery.Core.Catalog;

namespace MauiGallery.Features.Chrome;

/// <summary>A collapsible category in the sidebar.</summary>
public sealed partial class SidebarGroup(GalleryCategory category, IReadOnlyList<SidebarEntry> entries) : ObservableObject
{
    public GalleryCategory Category { get; } = category;

    public IReadOnlyList<SidebarEntry> Entries { get; } = entries;

    [ObservableProperty]
    public partial bool IsExpanded { get; set; }
}
