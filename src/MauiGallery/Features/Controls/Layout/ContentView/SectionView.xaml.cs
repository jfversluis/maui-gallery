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

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // The XAML source generator falls back to a reflection-based binding for RelativeSource TemplatedParent, which
        // isn't trim-safe, so the template's title binding is set here, compiled.
        if (GetTemplateChild("TitleLabel") is Label title)
        {
            title.SetBinding(Label.TextProperty, static (SectionView section) => section.Title, source: RelativeBindingSource.TemplatedParent);
        }
    }
}
