using System.Globalization;

namespace MauiGallery.Features.Fundamentals;

/// <summary>Formats a count with a noun passed as the ConverterParameter: "No photos", "1 photo", "3 photos".</summary>
public sealed class PluralConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var count = (int)Math.Round(System.Convert.ToDouble(value, culture));
        var noun = parameter as string ?? "item";
        return count switch
        {
            0 => $"No {noun}s",
            1 => $"1 {noun}",
            _ => string.Create(culture, $"{count:N0} {noun}s"),
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
