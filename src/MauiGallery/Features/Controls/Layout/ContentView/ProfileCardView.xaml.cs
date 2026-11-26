namespace MauiGallery.Features.Controls.Layouts;

/// <summary>A reusable profile card for the ContentView sample: an avatar with initials, a name and a role.</summary>
public sealed partial class ProfileCardView : ContentView
{
    public static readonly BindableProperty NameProperty =
        BindableProperty.Create(nameof(Name), typeof(string), typeof(ProfileCardView), string.Empty,
            propertyChanged: (bindable, _, _) => ((ProfileCardView)bindable).OnPropertyChanged(nameof(Initials)));

    public static readonly BindableProperty RoleProperty =
        BindableProperty.Create(nameof(Role), typeof(string), typeof(ProfileCardView), string.Empty);

    public static readonly BindableProperty AccentColorProperty =
        BindableProperty.Create(nameof(AccentColor), typeof(Color), typeof(ProfileCardView), Colors.Gray);

    public ProfileCardView() => InitializeComponent();

    public string Name
    {
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

    public string Role
    {
        get => (string)GetValue(RoleProperty);
        set => SetValue(RoleProperty, value);
    }

    public Color AccentColor
    {
        get => (Color)GetValue(AccentColorProperty);
        set => SetValue(AccentColorProperty, value);
    }

    /// <summary>The first letter of the first two words of <see cref="Name"/>.</summary>
    public string Initials =>
        string.Concat(Name.Split(' ', StringSplitOptions.RemoveEmptyEntries).Take(2).Select(word => char.ToUpperInvariant(word[0])));
}
