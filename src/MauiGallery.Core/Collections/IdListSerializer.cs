namespace MauiGallery.Core.Collections;

/// <summary>Stores lists of catalog identifiers as a single preference string.</summary>
public static class IdListSerializer
{
    private const char Separator = ',';

    public static string Serialize(IEnumerable<string> ids) => string.Join(Separator, ids);

    public static IReadOnlyList<string> Deserialize(string? value) =>
        string.IsNullOrWhiteSpace(value) ? [] : value.Split(Separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
