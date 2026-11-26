namespace MauiGallery.Features.Navigation;

/// <summary>
/// The detail page of the Shell sample. It is registered with a route template, so it receives the {color} path
/// parameter and the query string through <see cref="IQueryAttributable"/>. Not a catalog item.
/// </summary>
public sealed partial class ShellSwatchPage : ContentPage, IQueryAttributable
{
    public const string Route = "swatch/{color}";

    private static readonly Dictionary<string, Color> Colors = new(StringComparer.OrdinalIgnoreCase)
    {
        ["teal"] = Color.FromArgb("#0D9488"),
        ["orange"] = Color.FromArgb("#EA580C"),
        ["pink"] = Color.FromArgb("#DB2777"),
        ["blue"] = Color.FromArgb("#2563EB"),
    };

    public ShellSwatchPage() => InitializeComponent();

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        var color = query.TryGetValue("color", out var value) ? value as string ?? string.Empty : string.Empty;
        Swatch.Color = Colors.GetValueOrDefault(color, Microsoft.Maui.Graphics.Colors.Gray);
        ColorLabel.Text = $"color = \"{color}\"";
        NoteLabel.Text = query.TryGetValue("note", out var note) ? $"note = \"{note}\"" : "No note";
    }

    private async void OnSendBackClicked(object? sender, EventArgs e)
    {
        if (Window?.Page is Shell shell)
        {
            // ShellNavigationQueryParameters are delivered once, unlike a plain dictionary.
            await shell.GoToAsync("..", new ShellNavigationQueryParameters { ["reply"] = ReplyEntry.Text ?? string.Empty });
        }
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        if (Window?.Page is Shell shell)
        {
            await shell.GoToAsync("..");
        }
    }
}
