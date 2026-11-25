using System.Globalization;

namespace MauiGallery.DesignSystem.Converters;

/// <summary>Returns <see langword="true"/> when the value is a non-empty string or a non-null object.</summary>
public sealed class IsNotNullOrEmptyConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is string text ? !string.IsNullOrEmpty(text) : value is not null;

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
