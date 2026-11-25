# AGENTS.md

Guidance for AI coding agents (GitHub Copilot, Claude Code, Codex and others) working in this repository.
Humans should start with [README.md](README.md) and [CONTRIBUTING.md](CONTRIBUTING.md).

## What this repo is

The **.NET MAUI Gallery**: a showcase app for every .NET MAUI control, API and pattern, with live samples,
adjustable options and copyable source. It ships to the App Store, Mac App Store, Google Play and Microsoft Store,
so code quality and visual polish both matter.

## Repository map

| Path | Purpose |
| --- | --- |
| `src/MauiGallery` | The app (Android, iOS, Mac Catalyst, Windows). Feature folders under `Features/`. |
| `src/MauiGallery/DesignSystem` | Design tokens, keyed chrome styles and chrome components (`ControlExample`, `CodeViewer`, ...). |
| `src/MauiGallery/Features/Catalog` | `GalleryCatalog`: every page, its route, metadata and category. Routes and DI come from it. |
| `src/MauiGallery/Features/Chrome` | `AppShell` view model, sidebar and menu bar. |
| `src/MauiGallery/Features/Samples` | `SamplePage` base class, page header and related items. |
| `src/MauiGallery/Features/Controls/...` | One folder per sample page, with a `Snippets` folder for its code. |
| `src/MauiGallery.Core` | Platform-agnostic logic (catalog model, search, snippet templates, syntax highlighting). No MAUI dependency. |
| `tests/MauiGallery.Core.Tests` | xUnit v3 tests for `MauiGallery.Core` (Microsoft.Testing.Platform). |
| `.github/skills` | Agent skills from [dotnet/maui-labs](https://github.com/dotnet/maui-labs) (installed with `maui ai init`) and the repo's own `add-gallery-sample`. |
| `docs/` | Design system, agent setup, privacy policy. |
| `.maui/ai-assets.json` | Provenance of installed agent assets. Refresh with `maui ai update`. |

## Build, test, run

```bash
dotnet test tests/MauiGallery.Core.Tests                     # fast unit tests, run first
dotnet build src/MauiGallery -f net11.0-maccatalyst            # or net11.0-ios / net11.0-android
```

Warnings are errors (analyzers, code style, XAML compilation and trimming). Fix the cause; do not suppress
without a comment explaining why.

Run and inspect the app with DevFlow (the agent is compiled into Debug builds only):

```bash
# Mac Catalyst: -t:Run launches the last build and returns immediately, so build first
dotnet build src/MauiGallery -f net11.0-maccatalyst -t:Build\;Run
# iOS simulator: blocks for the app lifetime, run it in the background
dotnet build src/MauiGallery -f net11.0-ios -t:Build\;Run -p:_DeviceName=:v2:udid=<UDID>
# Android emulator: deploy, then connect DevFlow through adb
dotnet build src/MauiGallery -f net11.0-android -t:Run -p:AdbTarget="-s <serial>"
adb -s <serial> reverse tcp:19223 tcp:19223      # broker
adb -s <serial> forward tcp:<port> tcp:<port>    # agent port from `maui devflow list`

maui devflow wait --wait-platform <iOS|Android|MacCatalyst>
```

Use the DevFlow MCP tools (`maui_tree`, `maui_query`, `maui_tap`, `maui_navigate`, `maui_resize`,
`maui_layout_diagnostics`, `maui_logs`) to verify UI changes on a running app. Pages are reachable by route:
`maui devflow ui navigate "//main/controls/<item-id>" --agent-port <port>`.
Use a dedicated simulator/emulator named `MauiGallery-*`; other projects may share this machine. With several apps
connected, always pass `--agent-port` (CLI) or `agentPort` (MCP), and `--wait-platform` to `maui devflow wait`.

Known quirks:

- Screenshots: prefer `maui devflow ui screenshot --output file.png` (CLI). On Mac Catalyst the MCP tool can crop the
  window, and on Android it can return a stale image; `adb exec-out screencap -p` is a reliable fallback.
- Switch themes through the app (Settings, then Theme, AutomationIds `theme-light` / `theme-dark`), or system-wide
  with `xcrun simctl ui <udid> appearance dark` and `adb shell cmd uimode night yes`. The nuget.org DevFlow agent
  does not implement `maui_set_theme` or `maui_menu_list` yet.
- On Mac Catalyst the app uses the iPad idiom (`UIDeviceFamily` 2), scaled to 77%. The Mac idiom (6) is not an
  option: it throws on `UIStepper`.
- `Shell` itself never receives a layout size. Use `Window.SizeChanged` for window-level breakpoints.
- A XAML `ContentView` subclass with `[ContentProperty]` must wrap its own root content in
  `<ContentView.Content>`, or the content is assigned to the custom property (see `ControlExample.xaml`).
- Snippet files are embedded with `WithCulture="false"`: MSBuild otherwise reads `Name.cs.txt` as the Czech culture.
- Navigation failures are caught and logged by `ShellNavigationService`; check `maui_logs` when a page does not open.

## Architecture rules

- **Branded chrome, native stage.** The design system styles the chrome only, through keyed styles and custom
  components. Never add implicit (un-keyed) styles for core controls: samples must render with platform defaults.
- MVVM with `CommunityToolkit.Mvvm` source generators. Pages and view models are resolved through DI in
  `MauiProgram.cs`. No service locators, no `BuildServiceProvider()`.
- Every XAML file sets `x:DataType`; bindings are compiled (`MauiStrictXamlCompilation` is on).
- XAML uses .NET 11 implicit namespaces: no `xmlns` declarations for MAUI types. App namespaces are mapped to the
  global XAML namespace in `GlobalXmlns.cs`.
- Navigation uses Shell routes; receive parameters with `IQueryAttributable` (trim-safe), not `[QueryProperty]`.
- Keep platform code behind interfaces or in `Platforms/` partial classes; avoid `#if` in shared code where possible.
- Put logic that does not need MAUI in `MauiGallery.Core` and cover it with tests.

## C# conventions

File-scoped namespaces, `sealed` by default, primary constructors where they help, nullable enabled,
no `async void` except event handlers, `ConfigureAwait` not needed in app code. Chrome strings live in
`Resources/Strings/AppResources.resx`.

## Skills to load

| Task | Skill |
| --- | --- |
| Run, inspect, screenshot, fix UI | `maui-devflow-debug`, `devflow-automation` |
| DI, MVVM, Shell routing | `maui-app-architecture` |
| Layout, templates, state | `maui-ui-patterns`, `maui-controls-deep-dive` |
| Themes, RTL, localization | `maui-localization-theming` |
| Handlers and native views | `maui-custom-handlers`, `maui-platform-invoke` |
| Accessibility | `maui-accessibility` |
| Startup, trimming, AOT | `maui-performance` |
| Essentials and permissions | `maui-device-capabilities` |
| WebView, HybridWebView, Blazor | `maui-blazor-hybrid` |
| Deep links | `maui-notifications-deep-links` |
| Icons, splash, assets | `maui-app-assets-lifecycle`, `maui-project-structure` |
| Add a gallery page | `add-gallery-sample` (this repo's own workflow) |
