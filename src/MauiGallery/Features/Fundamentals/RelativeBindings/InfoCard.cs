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

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // The template's {Binding Title, Source={RelativeSource TemplatedParent}}, compiled: the XAML source generator
        // falls back to a reflection-based binding for TemplatedParent, which isn't trim-safe.
        if (GetTemplateChild("TitleLabel") is Label title)
        {
            title.SetBinding(Label.TextProperty, static (InfoCard card) => card.Title, source: RelativeBindingSource.TemplatedParent);
        }
    }
}
