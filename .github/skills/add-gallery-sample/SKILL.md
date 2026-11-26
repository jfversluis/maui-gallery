---
name: add-gallery-sample
description: >-
  Add or extend a sample page in the .NET MAUI Gallery repository. USE FOR: new control/API pages, new ControlExample
  cards, code snippets with $(Placeholder) substitutions, catalog entries, verifying a sample with DevFlow. DO NOT USE
  FOR: design-system or app-shell changes, generic MAUI questions outside this repo.
---

# Add a gallery sample

This repository-specific skill encodes how a sample page is built in the .NET MAUI Gallery. The human version of
this workflow is in `CONTRIBUTING.md`; the Button page (`src/MauiGallery/Features/Controls/BasicInput/Button`) is
the reference implementation. Read it before writing a new page.

## Workflow

1. **Research the control.** List its public properties and events, and how each platform renders it. Load
   `maui-current-apis` (and `maui-controls-deep-dive` for collections, gestures or graphics) before choosing APIs.
2. **Plan two to five examples**, simplest first. Each needs a title, a one-sentence description that explains why
   you would use it, and the options a reader can change.
3. **Create the page** in `Features/<Area>/<Category>/<Control>/<Control>Page.xaml` deriving from `SamplePage`, with
   `x:DataType="SamplePageViewModel"`, `ControlPageHeader`, one `ControlExample` per example, and `RelatedItems`.
   The code-behind constructor takes `SamplePageServices` and calls `InitializeComponent()`.
4. **Write snippets** in a `Snippets` folder: `<Control><Example>.xaml.txt` and `.cs.txt`, unique names across the
   app. Every `$(Name)` needs a `Substitution` with that `Key`; set `Default` so unchanged properties drop out, and
   `IsExpression="True"` for values that are C# code.
5. **Add the catalog entry** in the category's `Features/Catalog/GalleryCatalog.<Category>.cs`: kebab-case `Id`, one-sentence
   `Description`, `NativeMappings` for each platform, `RelatedIds`, docs, API and source URLs.
6. **Verify** (below). Do not report the sample as done until every check passes.

## Rules

- Samples render at **platform defaults**. Do not apply design-system styles or colors to the sample itself.
- Bind options straight to the sample: `{Binding Prop, Source={x:Reference Name}, x:DataType=Type}`.
- Report events through `ControlExample.Output`.
- `AutomationId`s: `sample-*` for sample controls, `option-*` for options.
- Keep the snippet identical in meaning to the live sample, minus gallery-only details.
- No new NuGet dependencies.

## Verify

```bash
dotnet test tests/MauiGallery.Core.Tests                    # snippets well-formed, names unique
dotnet build src/MauiGallery -f net11.0-maccatalyst -t:Build\;Run
maui devflow wait --wait-platform MacCatalyst
maui devflow ui navigate "//main/controls/<id>" --agent-port <port>
maui devflow ui screenshot --agent-port <port> --output <id>-light.png
```

Then, with the DevFlow MCP tools:

- `maui_layout_diagnostics` reports no clipping or overlap.
- Change each option with `maui_tap` / `maui_fill` and confirm with `maui_query` that both the sample and the
  `code-text` element changed.
- Repeat in dark mode (Settings, then Theme) and at a phone width (iOS simulator or Android emulator; see
  `AGENTS.md` for the Android port forwarding).
- `maui_logs` shows no errors. Navigation failures are logged by `ShellNavigationService`.
