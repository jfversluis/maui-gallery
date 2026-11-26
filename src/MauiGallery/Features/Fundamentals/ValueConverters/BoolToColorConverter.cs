using System.Globalization;

namespace MauiGallery.Features.Fundamentals;

/// <summary>Picks one of two colors for a boolean. The colors are properties, so XAML can configure each instance.</summary>
public sealed class BoolToColorConverter : IValueConverter
{
    public Color TrueColor { get; set; } = Colors.Green;

    public Color FalseColor { get; set; } = Colors.Gray;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is true ? TrueColor : FalseColor;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is Color color && color.Equals(TrueColor);
}
