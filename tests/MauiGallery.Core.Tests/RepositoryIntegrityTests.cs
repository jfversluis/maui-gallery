using System.Xml;
using System.Xml.Linq;
using MauiGallery.Core.Snippets;
using MauiGallery.Core.Syntax;

namespace MauiGallery.Core.Tests;

/// <summary>Checks the app's XAML files for mistakes that would otherwise only show up at runtime.</summary>
public sealed class RepositoryIntegrityTests
{
    private static readonly XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2009/xaml";

    public static TheoryData<string> SamplePages() =>
        [.. Directory.EnumerateFiles(Repository.Features, "*Page.xaml", SearchOption.AllDirectories)
            .Where(p => File.ReadAllText(p).Contains("<ControlExample", StringComparison.Ordinal))
            .Order(StringComparer.Ordinal)];

    [Fact]
    public void LightAndDarkThemesDefineTheSameTokens()
    {
        var light = Keys(Path.Combine(Repository.DesignSystem, "Tokens", "LightTheme.xaml"));
        var dark = Keys(Path.Combine(Repository.DesignSystem, "Tokens", "DarkTheme.xaml"));

        Assert.Equal(light.Order(StringComparer.Ordinal), dark.Order(StringComparer.Ordinal));
    }

    [Theory]
    [MemberData(nameof(SamplePages))]
    public void EverySnippetPlaceholderHasASubstitution(string page)
    {
        foreach (var example in LoadXaml(page).Descendants().Where(e => e.Name.LocalName == "ControlExample"))
        {
            var keys = example.Descendants()
                .Where(e => e.Name.LocalName == "Substitution")
                .Select(e => (string?)e.Attribute("Key"))
                .ToHashSet(StringComparer.Ordinal);

            foreach (var (attribute, language) in new[] { ("XamlSource", CodeLanguage.Xaml), ("CSharpSource", CodeLanguage.CSharp) })
            {
                if ((string?)example.Attribute(attribute) is not { } name)
                {
                    continue;
                }

                var snippet = Directory.EnumerateFiles(Repository.Features, name, SearchOption.AllDirectories).SingleOrDefault();
                Assert.True(snippet is not null, $"{Path.GetFileName(page)}: snippet '{name}' does not exist.");

                var missing = new SnippetTemplate(File.ReadAllText(snippet), language).Placeholders.Where(p => !keys.Contains(p)).ToList();
                Assert.True(missing.Count == 0, $"{Path.GetFileName(page)}: '{name}' uses {string.Join(", ", missing)} without a Substitution.");
            }
        }
    }

    [Fact]
    public void NoImplicitStylesInChrome()
    {
        var chrome = Directory.EnumerateFiles(Repository.DesignSystem, "*.xaml", SearchOption.AllDirectories)
            .Where(p => !p.EndsWith("SampleStyles.xaml", StringComparison.Ordinal));

        foreach (var file in chrome)
        {
            var implicitStyles = LoadXaml(file).Descendants()
                .Where(e => e.Name.LocalName == "Style" && e.Attribute(XamlNamespace + "Key") is null && e.Parent?.Name.LocalName is "ResourceDictionary" or "Resources")
                .ToList();

            Assert.True(implicitStyles.Count == 0, $"{Path.GetFileName(file)} defines an implicit style; chrome styles must be keyed so samples keep platform defaults.");
        }
    }

    /// <summary>Loads a XAML file that relies on .NET 11's implicit namespaces (no x: or default xmlns declarations).</summary>
    private static XDocument LoadXaml(string path)
    {
        var namespaces = new XmlNamespaceManager(new NameTable());
        namespaces.AddNamespace(string.Empty, "http://schemas.microsoft.com/dotnet/2021/maui");
        namespaces.AddNamespace("x", XamlNamespace.NamespaceName);
        var context = new XmlParserContext(null, namespaces, null, XmlSpace.None);
        using var reader = XmlReader.Create(path, new XmlReaderSettings(), context);
        return XDocument.Load(reader);
    }

    private static HashSet<string> Keys(string path) =>
        [.. LoadXaml(path).Root!.Elements().Select(e => (string?)e.Attribute(XamlNamespace + "Key")).OfType<string>()];

    private static class Repository
    {
        public static readonly string Root = FindRoot();

        public static string Features => Path.Combine(Root, "src", "MauiGallery", "Features");

        public static string DesignSystem => Path.Combine(Root, "src", "MauiGallery", "DesignSystem");

        private static string FindRoot()
        {
            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "MauiGallery.slnx")))
            {
                directory = directory.Parent;
            }

            return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
        }
    }
}
