using System.Globalization;

namespace MauiGallery.DesignSystem.Converters;

/// <summary>Rounds a slider value to a whole number, so code snippets show <c>8</c> instead of <c>8.0312</c>.</summary>
public sealed class RoundedConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is double number ? Math.Round(number) : value;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value;
}
