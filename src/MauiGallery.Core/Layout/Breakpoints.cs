namespace MauiGallery.Core.Layout;

public static class Breakpoints
{
    public const double Medium = 640;

    public const double Expanded = 1100;

    public static SizeClass Classify(double width) => width switch
    {
        >= Expanded => SizeClass.Expanded,
        >= Medium => SizeClass.Medium,
        _ => SizeClass.Compact,
    };
}
