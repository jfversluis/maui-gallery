using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Fundamentals;

public sealed partial class AccessibilityPage : SamplePage
{
    public AccessibilityPage(SamplePageServices services)
        : base(services)
    {
        InitializeComponent();
        DescribeExample.NativeTarget = FavoriteButton;
        HeadingExample.NativeTarget = RecipeTitle;
        AnnounceExample.NativeTarget = EmailEntry;
        HideExample.NativeTarget = RecipeCard;
        Loaded += (_, _) =>
        {
            OnDescribeChanged(this, EventArgs.Empty);
            OnHeadingChanged(this, EventArgs.Empty);
            OnHideChanged(this, EventArgs.Empty);
        };
    }

    // Reads the properties back from the button, the way a screen reader gets them.
    private void OnDescribeChanged(object? sender, EventArgs e)
    {
        var description = SemanticProperties.GetDescription(FavoriteButton);
        var hint = SemanticProperties.GetHint(FavoriteButton);
        var name = string.IsNullOrWhiteSpace(description) ? "Unlabeled" : description;
        DescribeExample.Output = string.IsNullOrWhiteSpace(hint) ? $"Reads: \"{name}, button\"" : $"Reads: \"{name}, button. {hint}\"";
    }

    private void OnFavoriteClicked(object? sender, EventArgs e)
    {
        var isFavorite = FavoriteIcon.Glyph == IconFont.Maui.FluentIcons.FluentIconsRegular.Heart24;
        FavoriteIcon.FontFamily = isFavorite ? IconFont.Maui.FluentIcons.FluentIconsFilled.FontFamily : IconFont.Maui.FluentIcons.FluentIconsRegular.FontFamily;
        FavoriteIcon.Glyph = isFavorite ? IconFont.Maui.FluentIcons.FluentIconsFilled.Heart24 : IconFont.Maui.FluentIcons.FluentIconsRegular.Heart24;
        DescribeExample.Output = isFavorite ? "Added to favorites" : "Removed from favorites";
    }

    private void OnHeadingChanged(object? sender, EventArgs e) =>
        HeadingExample.Output = $"Reads: \"{Heading(RecipeTitle)}\", then \"{Heading(RecipeSection)}\"";

    private static string Heading(Label label) => SemanticProperties.GetHeadingLevel(label) switch
    {
        SemanticHeadingLevel.None => label.Text,
        var level => $"{label.Text}, heading level {(int)level}",
    };

    private void OnSubscribeClicked(object? sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim();
        if (string.IsNullOrEmpty(email) || !email.Contains('@', StringComparison.Ordinal))
        {
            EmailError.Text = string.IsNullOrEmpty(email) ? "Enter your email address." : "Enter an email address like you@example.com.";
            EmailError.IsVisible = true;

            // Wait for the error to be laid out before moving the screen reader to it.
            Dispatcher.Dispatch(EmailError.SetSemanticFocus);
            AnnounceExample.Output = $"Moved semantic focus to \"{EmailError.Text}\"";
            return;
        }

        EmailError.IsVisible = false;
        var message = $"Subscribed {email}";
        SemanticScreenReader.Announce(message);
        AnnounceExample.Output = $"Announced \"{message}\"";
    }

    private void OnHideChanged(object? sender, EventArgs e)
    {
        var stops = new List<string>();
        CollectStops(RecipeCard, stops);

        // Collapses runs of the same stop, such as five star glyphs.
        var runs = stops.Aggregate(new List<(string Stop, int Count)>(), (list, stop) =>
        {
            if (list.Count > 0 && list[^1].Stop == stop)
            {
                list[^1] = (stop, list[^1].Count + 1);
            }
            else
            {
                list.Add((stop, 1));
            }

            return list;
        });
        HideExample.Output = $"Reads: {string.Join(" → ", runs.Select(r => r.Count == 1 ? r.Stop : $"{r.Stop} ×{r.Count}"))}";
    }

    // A simplified model of what a screen reader stops on: views with a description, or labels with text.
    private static void CollectStops(IView view, List<string> stops)
    {
        if (view is not BindableObject element || AutomationProperties.GetExcludedWithChildren(element) == true)
        {
            return;
        }

        if (AutomationProperties.GetIsInAccessibleTree(element) != false)
        {
            if (SemanticProperties.GetDescription(element) is { Length: > 0 } description)
            {
                stops.Add($"\"{description}\"");
                return;
            }

            if (view is Label { Text: { Length: > 0 } text })
            {
                stops.Add(text.All(c => char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.PrivateUse) ? "an icon glyph" : $"\"{text}\"");
                return;
            }
        }

        if (view is IContentView { PresentedContent: IView content })
        {
            CollectStops(content, stops);
        }
        else if (view is Layout layout)
        {
            foreach (var child in layout)
            {
                CollectStops(child, stops);
            }
        }
    }
}
