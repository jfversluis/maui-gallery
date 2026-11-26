using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using MauiGallery.Core.Catalog;
using MauiGallery.Core.Snippets;
using MauiGallery.Core.Syntax;

namespace MauiGallery.Core.Tests;

/// <summary>Checks the app's XAML files for mistakes that would otherwise only show up at runtime.</summary>
public sealed class RepositoryIntegrityTests
{
    private static readonly XNamespace XamlNamespace = "http://schemas.microsoft.com/winfx/2009/xaml";

    private static readonly bool[] DarkAndLight = [true, false];

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

    [Fact]
    public void EveryCustomViewHasAHandler()
    {
        // A class deriving straight from View has no handler unless the app registers one, and MAUI then throws
        // HandlerNotFoundException as soon as a page containing it is created.
        var app = Path.Combine(Repository.Root, "src", "MauiGallery");
        var sources = Directory.EnumerateFiles(app, "*.cs", SearchOption.AllDirectories)
            .Where(p => !p.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Select(File.ReadAllText)
            .ToList();
        var registrations = string.Concat(sources);

        var unregistered = sources
            .SelectMany(source => Regex.Matches(source, @"class\s+(\w+)\s*:\s*View\b").Select(m => m.Groups[1].Value))
            .Where(type => !registrations.Contains($"AddHandler<{type},", StringComparison.Ordinal))
            .ToList();

        Assert.True(unregistered.Count == 0, $"Register a handler for, or derive from a concrete view: {string.Join(", ", unregistered)}");
    }

    [Fact]
    public void EveryPlatformShotsPageHasItsImages()
    {
        var problems = new List<string>();
        foreach (var id in PlatformShots.ItemIds)
        {
            foreach (var platform in PlatformShots.Platforms)
            {
                foreach (var dark in DarkAndLight)
                {
                    var path = Path.Combine(Repository.PlatformShots, id, PlatformShots.FileNameFor(platform, dark));
                    if (!File.Exists(path))
                    {
                        problems.Add($"missing {Path.GetRelativePath(Repository.Root, path)}");
                    }
                    else if (!PlatformShots.TryReadPngSize(File.ReadAllBytes(path), out var width, out _) || width > 640)
                    {
                        problems.Add($"{Path.GetRelativePath(Repository.Root, path)} is not a PNG at most 640 pixels wide");
                    }
                }
            }
        }

        Assert.True(problems.Count == 0, $"Capture them with eng/platform-shots (see CONTRIBUTING.md):\n{string.Join('\n', problems)}");
    }

    [Fact]
    public void NoOrphanPlatformShots()
    {
        var expected = PlatformShots.ItemIds
            .SelectMany(id => PlatformShots.Platforms.SelectMany(p => DarkAndLight.Select(dark => Path.Combine(id, PlatformShots.FileNameFor(p, dark)))))
            .ToHashSet(StringComparer.Ordinal);
        var orphans = Directory.Exists(Repository.PlatformShots)
            ? Directory.EnumerateFiles(Repository.PlatformShots, "*", SearchOption.AllDirectories)
                .Select(p => Path.GetRelativePath(Repository.PlatformShots, p))
                .Where(p => !expected.Contains(p) && Path.GetFileName(p) != ".DS_Store")
                .ToList()
            : [];

        Assert.True(orphans.Count == 0, $"Images without a PlatformShots.ItemIds entry: {string.Join(", ", orphans)}");
    }

    [Fact]
    public void PlatformShotsPagesShowTheShowcase()
    {
        // Pairs each catalog id with its page type, then finds the page's XAML.
        var catalog = string.Concat(Directory.EnumerateFiles(Path.Combine(Repository.Features, "Catalog"), "GalleryCatalog.*.cs").Select(File.ReadAllText));
        var pages = Regex.Matches(catalog, @"Id = ""([a-z0-9-]+)"",.*?PageType = typeof\((\w+)\)", RegexOptions.Singleline)
            .ToDictionary(m => m.Groups[1].Value, m => m.Groups[2].Value, StringComparer.Ordinal);
        var showcasePages = Directory.EnumerateFiles(Repository.Features, "*Page.xaml", SearchOption.AllDirectories)
            .Where(p => File.ReadAllText(p).Contains("<PlatformShowcase", StringComparison.Ordinal))
            .Select(Path.GetFileNameWithoutExtension)
            .ToHashSet(StringComparer.Ordinal);

        var unknown = PlatformShots.ItemIds.Where(id => !pages.ContainsKey(id)).ToList();
        Assert.True(unknown.Count == 0, $"PlatformShots.ItemIds lists ids that aren't in the catalog: {string.Join(", ", unknown)}");

        var listed = PlatformShots.ItemIds.Select(id => pages[id]).ToHashSet(StringComparer.Ordinal);
        Assert.Equal(listed.Order(StringComparer.Ordinal), showcasePages.Order(StringComparer.Ordinal));
    }

    private static HashSet<string> Keys(string path) =>
        [.. LoadXaml(path).Root!.Elements().Select(e => (string?)e.Attribute(XamlNamespace + "Key")).OfType<string>()];

    private static class Repository
    {
        public static readonly string Root = FindRoot();

        public static string Features => Path.Combine(Root, "src", "MauiGallery", "Features");

        public static string DesignSystem => Path.Combine(Root, "src", "MauiGallery", "DesignSystem");

        public static string PlatformShots => Path.Combine(Root, "src", "MauiGallery", "Resources", "Raw", Core.Catalog.PlatformShots.Folder);

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
