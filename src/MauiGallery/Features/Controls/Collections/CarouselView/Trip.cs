using IconFont.Maui.FluentIcons;

namespace MauiGallery.Features.Controls.Collections;

/// <summary>A featured trip shown on a card in the CarouselView samples.</summary>
public sealed record Trip(string Title, string Details, string Glyph, Color Color);

/// <summary>The sample data for the CarouselView page.</summary>
public static class TripData
{
    public static IReadOnlyList<Trip> All { get; } =
    [
        new("Northern lights", "Tromsø · 4 nights", FluentIconsRegular.WeatherSnowflake24, Color.FromArgb("#1D4ED8")),
        new("Cherry blossoms", "Kyoto · 6 nights", FluentIconsRegular.LeafOne24, Color.FromArgb("#BE185D")),
        new("Safari", "Serengeti · 5 nights", FluentIconsRegular.AnimalPawPrint24, Color.FromArgb("#B45309")),
        new("Island hopping", "Cyclades · 8 nights", FluentIconsRegular.Beach24, Color.FromArgb("#0F766E")),
        new("Road trip", "Route 66 · 12 nights", FluentIconsRegular.VehicleCar24, Color.FromArgb("#6D28D9")),
        new("Glacier hike", "Patagonia · 7 nights", FluentIconsRegular.Backpack24, Color.FromArgb("#15803D")),
    ];
}
