using System.Collections.Concurrent;
using MauiGallery.Core.Snippets;

namespace MauiGallery.DesignSystem.Components;

/// <summary>Loads code snippet templates that are embedded next to each sample page (<c>*.xaml.txt</c>, <c>*.cs.txt</c>).</summary>
public static class EmbeddedSnippets
{
    private static readonly ConcurrentDictionary<string, SnippetTemplate> Cache = new(StringComparer.Ordinal);

    public static SnippetTemplate Get(string name) => Cache.GetOrAdd(name, static name =>
    {
        using var stream = typeof(EmbeddedSnippets).Assembly.GetManifestResourceStream(name)
            ?? throw new InvalidOperationException($"Code snippet '{name}' is not an embedded resource. Snippet files must end in .xaml.txt or .cs.txt.");
        using var reader = new StreamReader(stream);
        return new SnippetTemplate(reader.ReadToEnd());
    });
}
