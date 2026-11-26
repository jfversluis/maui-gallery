using System.Globalization;

namespace MauiGallery.Features.Fundamentals;

/// <summary>Turns a rating from 0 to 5 into a row of filled and empty stars.</summary>
public sealed class RatingToStarsConverter : IValueConverter
{
    private const int MaxStars = 5;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var stars = Math.Clamp((int)Math.Round(System.Convert.ToDouble(value, culture)), 0, MaxStars);
        return new string('★', stars) + new string('☆', MaxStars - stars);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
