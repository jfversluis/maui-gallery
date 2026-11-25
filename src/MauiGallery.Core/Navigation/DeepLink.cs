namespace MauiGallery.Core.Navigation;

/// <summary>Builds and parses links that open a gallery page, such as <c>mauigallery://item/button</c>.</summary>
public static class DeepLink
{
    public const string Scheme = "mauigallery";

    private static readonly string[] ItemHosts = ["item", "control"];

    public static Uri ForItem(string itemId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(itemId);
        return new Uri($"{Scheme}://item/{Uri.EscapeDataString(itemId)}");
    }

    /// <summary>Returns the item id from a gallery link, or <see langword="null"/> if the link is not one.</summary>
    public static string? TryGetItemId(Uri? uri)
    {
        if (uri is null || !uri.IsAbsoluteUri || !string.Equals(uri.Scheme, Scheme, StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        if (!ItemHosts.Contains(uri.Host, StringComparer.OrdinalIgnoreCase))
        {
            return null;
        }

        var id = Uri.UnescapeDataString(uri.AbsolutePath.Trim('/'));
        return id.Length == 0 || id.Contains('/', StringComparison.Ordinal) ? null : id.ToLowerInvariant();
    }
}
