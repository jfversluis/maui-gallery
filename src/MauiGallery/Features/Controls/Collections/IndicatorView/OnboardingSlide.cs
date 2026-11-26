using IconFont.Maui.FluentIcons;

namespace MauiGallery.Features.Controls.Collections;

/// <summary>A page of an onboarding tour, shown in the IndicatorView samples.</summary>
public sealed record OnboardingSlide(string Title, string Text, string Glyph, Color Color);

/// <summary>The sample data for the IndicatorView page.</summary>
public static class OnboardingData
{
    public static IReadOnlyList<OnboardingSlide> Slides { get; } =
    [
        new("Welcome", "Plan your next trip in minutes.", FluentIconsRegular.HandWave24, Color.FromArgb("#1D4ED8")),
        new("Discover", "Find places loved by locals.", FluentIconsRegular.Search24, Color.FromArgb("#0F766E")),
        new("Save", "Keep favorites for later.", FluentIconsRegular.Heart24, Color.FromArgb("#BE185D")),
        new("Share", "Plan together with friends.", FluentIconsRegular.People24, Color.FromArgb("#B45309")),
        new("Go", "Your trips work offline too.", FluentIconsRegular.Airplane24, Color.FromArgb("#6D28D9")),
    ];
}
