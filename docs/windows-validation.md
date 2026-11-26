# Validating the Windows app

The Windows app is validated by the [Windows UI workflow](../.github/workflows/windows-ui.yml). It builds the Debug
app (unpackaged, with the DevFlow agent), launches it and uses the `maui` CLI to visit every catalog page in light and
dark at three window widths: compact (520), medium (900) and expanded (1440). It uploads an artifact with:

| Path | Contents |
| --- | --- |
| `summary.md` | Agent connection, configurations, navigation failures, crashes (also shown on the run page) |
| `pages/<theme>-<size>/<id>.png` | DevFlow screenshot of every page's content |
| `review/<page>-<theme>-<width>.png` | Full window captures (title bar, Mica, sidebar): shell pages at every width, a few control pages at 1440, and web views again after they've loaded (`-late`) |
| `logs/` | DevFlow logs per configuration, log errors per page, navigation timestamps, the Windows event log, and the Debug build's `crash.log` (unhandled exceptions) and `first-chance.log` (every distinct exception, handled or not) |
| `diagnostics/` | DevFlow layout diagnostics per page, when the agent supports them |
| `desktop/` | Full-screen captures after launch and at the end |
| `build.binlog` | The MSBuild binary log |

The crawl itself is [`.github/scripts/windows-ui-crawl.ps1`](../.github/scripts/windows-ui-crawl.ps1). It reads the page
ids from `Features/Catalog`, so new pages are covered automatically. It is fail-soft: a page that fails to open or
crashes the app is recorded, the app is relaunched and the crawl goes on.

## Running it

The workflow runs on demand, and on pushes to the branch it was developed on (edit `on.push` to add others). Manual
runs need the workflow on the default branch; until then, change the `CRAWL_PAGES` and `CRAWL_CONFIGS` defaults at
the top of the workflow to crawl less on a push.

```bash
gh workflow run windows-ui.yml                                  # whole catalog, all six configurations
gh workflow run windows-ui.yml -f pages=button,entry -f configs=dark-compact
gh run watch                                                    # then:
gh run download <run id> --dir artifacts/windows
```

You can also run the script on any Windows machine with a desktop session, after building the Debug app and
installing the CLI (`dotnet tool install -g Microsoft.Maui.Cli --prerelease`):

```powershell
dotnet build src/MauiGallery -f net11.0-windows10.0.19041.0 -c Debug
maui devflow broker start
./.github/scripts/windows-ui-crawl.ps1 -AppPath <path to MauiGallery.exe> -OutDir artifacts/windows-ui
```

## Using your own Windows machine

GitHub-hosted `windows-latest` runners (Windows Server) have a desktop session, so the crawl runs there by default.
Their display has no transparency effects, so Mica shows its solid fallback color, and a cold WebView2 start takes
several seconds. To run it on a
Windows 11 machine instead, such as a Dev Box (for real Mica, a GPU, or a specific Windows build), register the
machine as a self-hosted runner with the `windows-ui` label.

1. In the repository, open **Settings > Actions > Runners > New self-hosted runner**, choose **Windows** and **x64**,
   and follow the download and `config.cmd` steps on the machine. When asked for labels, enter `windows-ui` (or add
   `--labels windows-ui` to `config.cmd`).
2. **Run the runner in your signed-in session (`run.cmd`), not as a Windows service.** Services run in session 0,
   which has no desktop: the app starts but no window is shown and screenshots fail. To start it automatically, add a
   shortcut to `run.cmd` to the Startup folder (`shell:startup`).
3. Keep the session unlocked. On a Dev Box or other remote machine, disconnecting Remote Desktop locks the session and
   the window stops rendering. Before you disconnect, hand the session to the console so it stays unlocked, from an
   elevated prompt: `tscon %sessionname% /dest:console`. Turn off the screen saver and screen lock for that account.
4. The workflow installs the .NET SDK from `global.json`, the MAUI workload and the `maui` CLI itself. The machine
   needs Git and PowerShell 7 (`winget install Microsoft.PowerShell`); Visual Studio is not required.

Then point the workflow at the runner, either for one run or for every run:

```bash
gh workflow run windows-ui.yml -f runner=windows-ui    # one run
gh variable set WINDOWS_UI_RUNNER --body windows-ui     # every run, including pushes; delete it to go back
```

The repository is public, so keep self-hosted runners away from untrusted code: this workflow only runs on manual
dispatch and on pushes to the repository's own branches, never on pull requests from forks. Don't add the
`windows-ui` label to workflows that run on `pull_request`.
