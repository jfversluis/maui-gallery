# .NET MAUI Gallery

**Explore every .NET MAUI control, API and pattern, with live samples and source.**

The .NET MAUI Gallery is the companion app for anyone getting started with .NET MAUI, in the spirit of the
[WinUI Gallery](https://github.com/microsoft/WinUI-Gallery). Every sample runs live on your device, with the
platform's own native controls. Change a sample's options, watch its XAML and C# update, and copy the code into
your app.

It runs on **iOS, Android, macOS (Mac Catalyst) and Windows**, with layouts designed for each: bottom tabs on
phones, and a docked sidebar, menu bar, keyboard shortcuts and multiple windows on desktop.

## Highlights

- **Native by design.** Samples render at platform defaults on a neutral stage. A badge shows the native control
  each one became (`UIButton`, `MaterialButton`, ...), read at runtime from its handler. Toggle a branded style or
  right-to-left layout per sample.
- **Live code.** XAML and C# for every example, updated as you change options, with syntax highlighting and copy.
- **Built like a real app.** .NET 11, MVVM with CommunityToolkit.Mvvm, dependency injection, compiled bindings,
  source-generated XAML, trimming-safe code, unit tests and CI for all four platforms.
- **Agent-ready.** `AGENTS.md`, .NET MAUI agent skills and DevFlow let AI agents build, run and verify the app. See
  [docs/agents.md](docs/agents.md).

## Build and run

Requirements: .NET SDK 11.0.100+ with the `maui` workload, plus Xcode 27.2 for Apple platforms or the Android SDK.

```bash
dotnet workload install maui
dotnet test tests/MauiGallery.Core.Tests
dotnet build src/MauiGallery -f net11.0-maccatalyst -t:Build\;Run
```

## Repository layout

```text
src/MauiGallery          The app: design system, app chrome and one folder per sample page
src/MauiGallery.Core     Platform-agnostic logic: catalog model, search, snippets, syntax highlighting, deep links
tests/                   Unit tests
docs/                    Design system, agent setup, privacy policy
```

## Contributing

New samples are very welcome. [CONTRIBUTING.md](CONTRIBUTING.md) walks through adding a page in four steps.

## License

[MIT](LICENSE). Third-party fonts and icons are listed in the app under Settings, Third-party notices.
