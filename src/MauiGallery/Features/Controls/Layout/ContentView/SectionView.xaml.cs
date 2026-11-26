namespace MauiGallery.Features.Controls.Layouts;

/// <summary>A titled group for the ContentView sample that wraps any content in its own chrome.</summary>
public sealed partial class SectionView : ContentView
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(SectionView), string.Empty);

    public SectionView() => InitializeComponent();

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}
