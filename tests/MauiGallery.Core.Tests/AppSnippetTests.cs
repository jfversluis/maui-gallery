using System.Xml.Linq;
using MauiGallery.Core.Snippets;
using MauiGallery.Core.Syntax;

namespace MauiGallery.Core.Tests;

/// <summary>Checks the code snippets that ship with the app's sample pages.</summary>
public sealed class AppSnippetTests
{
    private const string Namespaces =
        """xmlns="http://schemas.microsoft.com/dotnet/2021/maui" xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml" xmlns:icons="icons" xmlns:local="local" """;

    public static TheoryData<string> XamlSnippets() => [.. SnippetFiles("*.xaml.txt")];

    public static TheoryData<string> AllSnippets() => [.. SnippetFiles("*.xaml.txt").Concat(SnippetFiles("*.cs.txt"))];

    [Theory]
    [MemberData(nameof(XamlSnippets))]
    public void XamlSnippetIsWellFormedWithValues(string path) => AssertWellFormed(path, _ => "Value");

    [Theory]
    [MemberData(nameof(XamlSnippets))]
    public void XamlSnippetIsWellFormedWithDefaultsOmitted(string path) => AssertWellFormed(path, _ => null);

    [Theory]
    [MemberData(nameof(AllSnippets))]
    public void SnippetFileNamesAreUnique(string path)
    {
        var name = Path.GetFileName(path);

        Assert.Single(SnippetFiles("*.txt"), p => Path.GetFileName(p) == name);
    }

    private static void AssertWellFormed(string path, Func<string, string?> value)
    {
        var template = new SnippetTemplate(File.ReadAllText(path), CodeLanguage.Xaml);
        var rendered = template.Render(template.Placeholders.ToDictionary(p => p, value));

        var exception = Record.Exception(() => XDocument.Parse($"<Root {Namespaces}>{rendered}</Root>"));

        Assert.True(exception is null, $"{Path.GetFileName(path)} is not well-formed XAML: {exception?.Message}");
    }

    private static IEnumerable<string> SnippetFiles(string pattern)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "MauiGallery.slnx")))
        {
            directory = directory.Parent;
        }

        var features = Path.Combine(directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found."), "src", "MauiGallery", "Features");
        return Directory.EnumerateFiles(features, pattern, SearchOption.AllDirectories).Order(StringComparer.Ordinal);
    }
}
