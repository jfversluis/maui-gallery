using System.Globalization;

namespace MauiGallery.Features.Fundamentals;

/// <summary>Combines the values of a MultiBinding: true only when every one of them is true.</summary>
public sealed class AllTrueConverter : IMultiValueConverter
{
    public object? Convert(object?[] values, Type targetType, object? parameter, CultureInfo culture) =>
        values.All(value => value is true);

    public object?[] ConvertBack(object? value, Type[] targetTypes, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
