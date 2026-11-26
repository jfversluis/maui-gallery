namespace MauiGallery.Features.Fundamentals;

/// <summary>A titled card whose look comes entirely from a ControlTemplate, for the TemplatedParent sample.</summary>
public sealed class InfoCard : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(InfoCard), string.Empty);

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}
