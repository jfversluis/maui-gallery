# Contributing to the .NET MAUI Gallery

Thanks for helping new .NET MAUI developers discover the framework. This guide covers the one thing most
contributions do: **adding a sample page**. For the visual language, see [docs/design-system.md](docs/design-system.md).

## Prerequisites

- .NET SDK 11.0.100 or later (see `global.json`) with the `maui` workload: `dotnet workload install maui`
- Xcode 27.2 for iOS and Mac Catalyst; Android SDK for Android; Windows for the Windows target
- Optional, for verifying UI: the `maui` CLI (`dotnet tool install -g Microsoft.Maui.Cli --prerelease`), which provides DevFlow

```bash
dotnet test tests/MauiGallery.Core.Tests            # fast, no device needed
dotnet build src/MauiGallery -f net11.0-maccatalyst  # or -ios, -android, -windows10.0.19041.0
```

## Add a sample page

Every page in the app is an entry in the catalog plus a page that derives from `SamplePage`. The `Button` page,
`src/MauiGallery/Features/Controls/BasicInput/Button`, is the reference implementation: copy its structure.

### 1. Create the page

Create a folder per control under the category, for example `Features/Controls/BasicInput/Switch/`:

```xml
<!-- SwitchPage.xaml -->
<SamplePage x:Class="MauiGallery.Features.Controls.BasicInput.SwitchPage"
            x:DataType="SamplePageViewModel">
    <ScrollView>
        <VerticalStackLayout Style="{StaticResource PageContent}">
            <ControlPageHeader />

            <ControlExample Title="A simple switch"
                            Description="One sentence on what this example shows and when to use it."
                            XamlSource="SwitchBasic.xaml.txt"
                            CSharpSource="SwitchBasic.cs.txt">
                <Switch x:Name="SampleSwitch" IsToggled="True" />
                <ControlExample.Options>
                    <!-- Native controls that change the sample. Bind straight to the sample with x:Reference. -->
                </ControlExample.Options>
                <ControlExample.Substitutions>
                    <!-- One per $(Placeholder) in the snippets. -->
                </ControlExample.Substitutions>
            </ControlExample>

            <RelatedItems />
        </VerticalStackLayout>
    </ScrollView>
</SamplePage>
```

```csharp
// SwitchPage.xaml.cs
using MauiGallery.Features.Samples;

namespace MauiGallery.Features.Controls.BasicInput;

public sealed partial class SwitchPage : SamplePage
{
    public SwitchPage(SamplePageServices services)
        : base(services) => InitializeComponent();
}
```

Guidelines:

- **Samples use platform defaults.** Never style the sample itself unless the example is about styling. The stage's
  Styled toggle shows a branded look for you.
- Order examples from the simplest to the most advanced. Two to five examples is typical.
- Each example gets a title and a one-sentence description that explains *why*, not just *what*.
- Options bind directly to the sample with `{Binding ..., Source={x:Reference Name}, x:DataType=Type}`. Use
  code-behind only when a value is composite (see the image position option on the Button page).
- Raise `Output` for events (`MyExample.Output = "Toggled on"`), so readers see events happen.
- If the sample is a layout, set `NativeTarget` to the control the native badge should describe.
- Give interactive sample controls an `AutomationId` starting with `sample-`, and options `option-`.

### 2. Add the code snippets

Put snippets in a `Snippets` folder next to the page. File names must be unique across the app:
`<Control><Example>.xaml.txt` and `<Control><Example>.cs.txt`. They are embedded automatically.

```xml
<Switch
    IsToggled="$(IsToggled)"
    OnColor="$(OnColor)" />
```

Each `$(Name)` needs a `Substitution` with the same `Key`:

| Property | Meaning |
| --- | --- |
| `Value` | The live value, usually bound to an option control. |
| `Default` | When `Value` equals this, the whole line is left out, so code only shows what the reader changed. |
| `IsExpression` | For C#: insert the value as code (`Button.ButtonContentLayout.ImagePosition.Top`), not as a string. |

Booleans, numbers and colors are formatted for XAML and C# automatically.

### 3. Register it in the catalog

Add an entry to `Features/Catalog/GalleryCatalog.Data.cs`. Routing, dependency injection, navigation, search and
deep links (`mauigallery://item/{id}`) all come from this entry.

```csharp
new()
{
    Id = "switch",                                  // kebab-case: also the route and deep-link id
    Title = "Switch",
    Description = "Turns an option on or off.",     // one sentence, shown on cards and in search
    CategoryId = CategoryIds.BasicInput,
    IconGlyph = FluentIconsRegular.ToggleLeft24,
    PageType = typeof(SwitchPage),
    Keywords = ["toggle", "on", "off"],
    NativeMappings = [new(GalleryPlatforms.iOS, "UISwitch"), /* ... */],
    RelatedIds = ["checkbox"],
    DocsUrl = new(DocsRoot + "user-interface/controls/switch"),
    ApiUrl = new(ApiRoot + "microsoft.maui.controls.switch"),
    SourceUrl = new(SourceRoot + "Controls/BasicInput/Switch"),
},
```

The catalog is validated at startup (unique kebab-case ids, known categories, valid related items).

### 4. Verify

```bash
dotnet test tests/MauiGallery.Core.Tests   # every snippet must be well-formed XAML, names unique
```

Then run the app and check the page on at least one mobile and one desktop target, in light and dark:

- The page opens from the sidebar, from search and from `mauigallery://item/{id}`.
- Every option changes the sample **and** the code.
- No clipped or overlapping content at phone width and at a wide desktop window.

With DevFlow: `maui devflow ui navigate "//main/controls/{id}"`, then `maui devflow ui screenshot`, and the
`maui_layout_diagnostics` MCP tool. AI agents can follow the `add-gallery-sample` skill in `.github/skills`.

## Code conventions

The build treats warnings as errors and enforces code style, so `dotnet build` is the first reviewer.
[AGENTS.md](AGENTS.md) lists the architecture rules; the short version:

- MVVM with CommunityToolkit.Mvvm source generators, constructor injection, no service locators.
- `x:DataType` on every page, view and template.
- File-scoped namespaces, `sealed` classes, nullable reference types.
- Chrome uses design-system resources; samples never do.
- Logic without a MAUI dependency goes in `MauiGallery.Core`, with tests.

## Commits and pull requests

Keep commits focused and describe *why*. Include screenshots for UI changes (light and dark, phone and desktop).
