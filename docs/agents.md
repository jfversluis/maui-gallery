# Working on this repo with AI agents

This repository doubles as an example of how to set up a .NET MAUI app for AI-assisted development. Everything below
is plain files in the repo, so it works the same for every contributor and every agent.

## What is configured

| File | Read by | Purpose |
| --- | --- | --- |
| [`AGENTS.md`](../AGENTS.md) | Copilot, Codex, Claude (via `CLAUDE.md`) and others | Repo map, build/test/run commands, architecture rules, known quirks |
| [`.github/copilot-instructions.md`](../.github/copilot-instructions.md) | GitHub Copilot | Points to `AGENTS.md`; asks for runtime verification |
| [`.github/instructions/*.instructions.md`](../.github/instructions) | GitHub Copilot | Rules that apply to `*.xaml` and `*.cs` files |
| [`.github/skills/`](../.github/skills) | Copilot CLI, VS Code | Task-specific skills: 22 from dotnet/maui-labs, plus `add-gallery-sample` |
| [`.vscode/mcp.json`](../.vscode/mcp.json) | VS Code | Registers the `maui-devflow` MCP server |
| [`.maui/ai-assets.json`](../.maui/ai-assets.json) | `maui ai` CLI | Provenance (source repo and commit) of installed assets |

## Why these pieces

- **`AGENTS.md` is the single source of truth.** Tool-specific files point to it instead of repeating it, so they
  cannot drift apart.
- **Strict builds are guardrails.** Warnings are errors, code style is enforced at build time, XAML bindings are
  compiled, and the trim analyzer is on. An agent gets precise, local feedback from `dotnet build` instead of
  discovering problems at runtime.
- **Testable logic lives in `MauiGallery.Core`.** Search, snippet rendering, syntax highlighting and deep links run
  in `dotnet test` in about a second, without a device.
- **DevFlow closes the loop.** The DevFlow agent is compiled into Debug builds only. With the `maui-devflow` MCP
  server, an agent can launch the app, read the visual tree, tap, resize the window, switch themes, take
  screenshots and run layout diagnostics: it can check its own UI work.
- **Skills carry expertise.** Skills load only when relevant, so they add MAUI knowledge without bloating every
  prompt. `add-gallery-sample` captures this repo's own workflow.

## Set it up yourself

```bash
dotnet tool install -g Microsoft.Maui.Cli --prerelease

# Install skills and the DevFlow MCP server for Copilot CLI and VS Code
maui ai init --env CopilotCli --env VsCode --skill maui-devflow-debug --skill maui-app-architecture --yes

# See what is installed and whether updates are available; refresh later
maui ai status
maui ai update
```

Use `maui ai list skill` to browse the catalog. For Claude Code, add `--env Claude` (skills go to `.claude/skills`
and the MCP server to `.mcp.json`).

To add DevFlow to your own app, reference `Microsoft.Maui.DevFlow.Agent` for Debug builds only and call
`builder.AddMauiDevFlowAgent()` inside `#if DEBUG`; on Mac Catalyst, add the
`com.apple.security.network.server` entitlement to Debug builds. See `src/MauiGallery/MauiGallery.csproj` and
`MauiProgram.cs`.

## Tips for prompting

- Name the outcome and the verification: "Add a Switch page following `add-gallery-sample`, and verify it in light
  and dark on the iOS simulator and Mac Catalyst."
- Ask for screenshots from DevFlow as evidence, not just a successful build.
- Keep each agent on its own simulator or emulator (`MauiGallery-*` devices) when several run at once.
