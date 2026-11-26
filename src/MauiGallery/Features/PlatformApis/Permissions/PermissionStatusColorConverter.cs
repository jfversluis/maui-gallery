using System.Globalization;

namespace MauiGallery.Features.PlatformApis;

/// <summary>Colors a permission status chip. Every color keeps white text readable in light and dark themes.</summary>
public sealed class PermissionStatusColorConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => value switch
    {
        PermissionStatus.Granted => Color.FromArgb("#15803D"),
        PermissionStatus.Limited => Color.FromArgb("#1D4ED8"),
        PermissionStatus.Denied => Color.FromArgb("#B91C1C"),
        PermissionStatus.Restricted or PermissionStatus.Disabled => Color.FromArgb("#B45309"),
        _ => Color.FromArgb("#6B7280"),
    };

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
