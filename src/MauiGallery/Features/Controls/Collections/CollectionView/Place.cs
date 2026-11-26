using IconFont.Maui.FluentIcons;

namespace MauiGallery.Features.Controls.Collections;

/// <summary>A travel destination shown in the CollectionView samples.</summary>
public sealed record Place(string Name, string Country, string Continent, string Glyph, Color Color);

/// <summary>Places on one continent, for the grouping sample. A group is a list of its items plus a header.</summary>
public sealed class PlaceGroup(string name, IEnumerable<Place> places) : List<Place>(places)
{
    public string Name { get; } = name;
}

/// <summary>The sample data for the CollectionView page.</summary>
public static class PlaceData
{
    private static readonly Color Blue = Color.FromArgb("#2563EB");
    private static readonly Color Teal = Color.FromArgb("#0D9488");
    private static readonly Color Amber = Color.FromArgb("#D97706");
    private static readonly Color Rose = Color.FromArgb("#E11D48");
    private static readonly Color Violet = Color.FromArgb("#7C3AED");
    private static readonly Color Green = Color.FromArgb("#16A34A");

    public static IReadOnlyList<Place> All { get; } =
    [
        new("Lisbon", "Portugal", "Europe", FluentIconsRegular.WeatherSunnyHigh24, Amber),
        new("Naples", "Italy", "Europe", FluentIconsRegular.FoodPizza24, Rose),
        new("Bergen", "Norway", "Europe", FluentIconsRegular.VehicleShip24, Blue),
        new("Amsterdam", "Netherlands", "Europe", FluentIconsRegular.VehicleBicycle24, Violet),
        new("Kyoto", "Japan", "Asia", FluentIconsRegular.LeafOne24, Green),
        new("Seoul", "South Korea", "Asia", FluentIconsRegular.Food24, Rose),
        new("Hanoi", "Vietnam", "Asia", FluentIconsRegular.DrinkCoffee24, Amber),
        new("Banff", "Canada", "Americas", FluentIconsRegular.Tent24, Teal),
        new("New York", "United States", "Americas", FluentIconsRegular.BuildingMultiple24, Blue),
        new("Cusco", "Peru", "Americas", FluentIconsRegular.Backpack24, Amber),
        new("Cape Town", "South Africa", "Africa", FluentIconsRegular.BuildingLighthouse24, Teal),
        new("Marrakesh", "Morocco", "Africa", FluentIconsRegular.Cart24, Rose),
        new("Melbourne", "Australia", "Oceania", FluentIconsRegular.DrinkCoffee24, Violet),
        new("Queenstown", "New Zealand", "Oceania", FluentIconsRegular.Airplane24, Green),
    ];

    public static IReadOnlyList<PlaceGroup> ByContinent { get; } =
        [.. All.GroupBy(p => p.Continent).Select(g => new PlaceGroup(g.Key, g))];
}
