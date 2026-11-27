<#
.SYNOPSIS
    Crawls every gallery page on Windows with DevFlow and captures screenshots, logs and diagnostics.

.DESCRIPTION
    Launches the Debug build of the app (unpackaged), waits for the DevFlow agent and then, for every theme and
    window width, navigates to each catalog route and takes a screenshot. It is fail-soft: navigation failures,
    screenshot failures and crashes are recorded in summary.md and the crawl goes on, relaunching the app if needed.

    Output layout (OutDir):
      pages/<config>/<id>.png    DevFlow screenshot of the page content (or a window capture if DevFlow failed)
      review/<page>-<theme>-<width>.png Window captures (title bar, Mica, sidebar) for review
      logs/                      DevFlow logs per configuration, log errors per page, Windows event log
      diagnostics/               Layout diagnostics per page, when the agent supports them
      summary.md                 What worked and what failed

    With -Mode PlatformShots it instead captures the "Same code, every platform" images: for every page listed in
    src/MauiGallery.Core/Catalog/PlatformShots.cs, in light and dark, a DevFlow screenshot of the window with the
    first example's sample on screen and the dot grid hidden, saved with the sample's bounds in the window to
      platform-shots/raw/windows/<id>-<theme>.png and .json
    Normalize them with eng/platform-shots/normalize.py.

    With -Mode Repeat it hunts intermittent crashes instead: it opens each page in -Pages -Repeat times, coming from
    -RepeatFrom each time and leaving again after a random delay of up to -RepeatDelay seconds (so some visits leave a
    page while it is still loading), and relaunches the app every -RelaunchEvery visits so cold starts are covered too.
    -Variants runs the whole set once per variant: a semicolon-separated list of comma-separated NAME=VALUE environment
    variables for the app process ("default" for none), for A/B experiments with Debug-only switches.
#>
param(
    [Parameter(Mandatory)] [string] $AppPath,
    [Parameter(Mandatory)] [string] $OutDir,
    [string] $CatalogDir = 'src/MauiGallery/Features/Catalog',
    # Comma-separated page ids; empty crawls the whole catalog.
    [string] $Pages = '',
    # Comma-separated <theme>-<size> configurations.
    [string] $Configs = 'light-expanded,dark-expanded,light-medium,dark-medium,light-compact,dark-compact',
    [double] $PageDelay = 2.0,
    # Crawl (screenshots of every page) or PlatformShots (the "Same code, every platform" captures).
    [ValidateSet('Crawl', 'PlatformShots', 'Repeat')] [string] $Mode = 'Crawl',
    # Repeat mode: visits per page and variant, the page visited in between, the longest stay and the cold starts.
    [int] $Repeat = 20,
    [string] $RepeatFrom = 'button',
    # Repeat mode: what to do before each visit, comma-separated: page ids, home, controls, settings, theme (switch
    # between light and dark) or resize (switch between the expanded and medium widths). Defaults to -RepeatFrom.
    [string] $RepeatPath = '',
    [double] $RepeatDelay = 3.0,
    [int] $RelaunchEvery = 5,
    [string] $Variants = 'default'
)

$ErrorActionPreference = 'Continue'
$sizes = @{
    compact  = @(520, 860)
    medium   = @(900, 860)
    expanded = @(1440, 900)
}
# Window captures (with the title bar and backdrop) for review: the shell pages in every configuration, and pages that
# show the WinUI controls well at the expanded width.
$chromeRoutes = [ordered]@{
    home     = '//main/home'
    controls = '//main/controls'
    settings = '//main/settings'
    button   = '//main/controls/button'
}
$reviewRoutes = [ordered]@{
    'collection-view' = '//main/controls/collection-view'
    'graphics-view'   = '//main/controls/graphics-view'
    'date-picker'     = '//main/controls/date-picker'
    'entry'           = '//main/controls/entry'
    'slider'          = '//main/controls/slider'
    'menu-bar'        = '//main/controls/menu-bar'
    'hybrid-web-view' = '//main/controls/hybrid-web-view'
    'web-view'        = '//main/controls/web-view'
    'blazor-web-view' = '//main/controls/blazor-web-view'
}

foreach ($dir in 'pages', 'review', 'logs', 'diagnostics', 'desktop') {
    New-Item -ItemType Directory -Force -Path (Join-Path $OutDir $dir) | Out-Null
}
$summary = Join-Path $OutDir 'summary.md'
$logErrors = Join-Path $OutDir 'logs/log-errors.txt'
$failures = [System.Collections.Generic.List[string]]::new()
Set-Content $summary "# Windows UI crawl`n`nStarted $(Get-Date -Format o)`n"

function Write-Summary([string] $line) {
    Write-Host $line
    Add-Content $summary $line
}

function Add-Failure([string] $line) {
    $failures.Add($line)
    Write-Host "::warning::$line"
}

#region Native window capture
Add-Type -AssemblyName System.Drawing
Add-Type -TypeDefinition @'
using System;
using System.Runtime.InteropServices;

public static class Win32Window
{
    [StructLayout(LayoutKind.Sequential)] public struct RECT { public int Left, Top, Right, Bottom; }

    [DllImport("user32.dll")] public static extern bool SetProcessDPIAware();
    [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    [DllImport("user32.dll")] static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);
    [DllImport("dwmapi.dll")] static extern int DwmGetWindowAttribute(IntPtr hWnd, int attribute, out RECT rect, int size);
    [DllImport("user32.dll")] public static extern int GetSystemMetrics(int index);

    public static RECT Bounds(IntPtr hWnd)
    {
        RECT r;
        // DWMWA_EXTENDED_FRAME_BOUNDS excludes the invisible resize borders.
        if (DwmGetWindowAttribute(hWnd, 9, out r, Marshal.SizeOf(typeof(RECT))) != 0) GetWindowRect(hWnd, out r);
        return r;
    }
}
'@
[Win32Window]::SetProcessDPIAware() | Out-Null

function Get-ScreenSize { "$([Win32Window]::GetSystemMetrics(0))x$([Win32Window]::GetSystemMetrics(1))" }

function Get-WindowBounds([IntPtr] $handle) {
    $r = [Win32Window]::Bounds($handle)
    "$($r.Left),$($r.Top) $($r.Right - $r.Left)x$($r.Bottom - $r.Top)"
}

function Save-ScreenRegion([int] $x, [int] $y, [int] $width, [int] $height, [string] $path) {
    $bitmap = [System.Drawing.Bitmap]::new([Math]::Max($width, 1), [Math]::Max($height, 1))
    try {
        $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
        $graphics.CopyFromScreen($x, $y, 0, 0, $bitmap.Size)
        $graphics.Dispose()
        $bitmap.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    } finally {
        $bitmap.Dispose()
    }
}

function Save-Screen([string] $path) {
    try {
        Save-ScreenRegion 0 0 ([Win32Window]::GetSystemMetrics(0)) ([Win32Window]::GetSystemMetrics(1)) $path
    } catch {
        Write-Host "::warning::Screen capture failed: $_"
    }
}

function Save-WindowRegion([IntPtr] $handle, [string] $path) {
    [Win32Window]::ShowWindow($handle, 9) | Out-Null # SW_RESTORE
    [Win32Window]::SetForegroundWindow($handle) | Out-Null
    Start-Sleep -Milliseconds 300
    $r = [Win32Window]::Bounds($handle)
    Save-ScreenRegion $r.Left $r.Top ($r.Right - $r.Left) ($r.Bottom - $r.Top) $path
}
#endregion

#region DevFlow helpers
$script:port = $null
$script:proc = $null
$script:appEnvironment = @{}

function Invoke-DevFlow {
    param([Parameter(ValueFromRemainingArguments)] [string[]] $Arguments)
    if ($Arguments[0] -eq 'ui' -and $Arguments[1] -eq 'navigate') {
        Add-Content (Join-Path $OutDir 'logs/navigation.txt') "[$(Get-Date -Format o)] $($Arguments[2])"
    }
    $all = @('devflow') + $Arguments
    if ($script:port) { $all += @('--agent-port', $script:port) }
    $output = & maui @all 2>&1 | Out-String
    [pscustomobject]@{ Ok = ($LASTEXITCODE -eq 0); Output = $output.Trim() }
}

function Start-App {
    Write-Host "Launching $AppPath"
    $script:proc = Start-Process -FilePath $AppPath -WorkingDirectory (Split-Path $AppPath) -PassThru -Environment $script:appEnvironment
    $script:port = $null
    $deadline = (Get-Date).AddSeconds(180)
    while ((Get-Date) -lt $deadline) {
        if ($script:proc.HasExited) {
            Add-Failure "App exited during startup (exit code $($script:proc.ExitCode))"
            return $false
        }
        $agents = & maui devflow list --json 2>$null | Out-String
        try {
            $agent = @($agents | ConvertFrom-Json) | ForEach-Object { $_ } | Where-Object { $_.tfm -like '*windows*' } | Select-Object -First 1
        } catch { $agent = $null }
        if ($agent) {
            $script:port = [string]$agent.port
            Write-Summary "- Agent connected: platform $($agent.platform), port $($agent.port), version $($agent.version)"
            break
        }
        Start-Sleep -Seconds 2
    }
    if (-not $script:port) {
        Add-Failure 'DevFlow agent did not register with the broker within 180 s'
        Write-Summary "- maui devflow list: $(& maui devflow list 2>&1 | Out-String)"
        return $false
    }

    # The main window handle appears once WinUI has created the window.
    for ($i = 0; $i -lt 30 -and $script:proc.MainWindowHandle -eq 0; $i++) {
        Start-Sleep -Seconds 1
        $script:proc.Refresh()
    }
    $status = Invoke-DevFlow agent status
    Write-Summary "- Agent status ok: $($status.Ok)"
    Start-Sleep -Seconds 3
    return $true
}

function Test-AppAlive {
    $script:proc.Refresh()
    return -not $script:proc.HasExited
}

function Save-WindowCapture([string] $path) {
    $script:proc.Refresh()
    if ($script:proc.MainWindowHandle -eq 0) { return $false }
    try {
        Save-WindowRegion $script:proc.MainWindowHandle $path
        return $true
    } catch {
        Add-Failure "Window capture failed for ${path}: $_"
        return $false
    }
}

function Save-PageScreenshot([string] $path) {
    $shot = Invoke-DevFlow ui screenshot --output $path --overwrite
    if ($shot.Ok -and (Test-Path $path)) { return 'devflow' }
    if (-not $script:screenshotFailureLogged) {
        $script:screenshotFailureLogged = $true
        Add-Failure "DevFlow screenshot failed, falling back to window captures: $($shot.Output)"
    }
    if (Save-WindowCapture $path) { return 'window' }
    return $null
}

function Set-Configuration([string] $theme, [string] $size) {
    $nav = Invoke-DevFlow ui navigate '//main/settings'
    Start-Sleep -Seconds 1
    $tap = Invoke-DevFlow ui tap "theme-$theme"
    if (-not $tap.Ok) { Add-Failure "Theme $theme could not be applied: $($tap.Output)" }
    $w, $h = $sizes[$size]
    $resize = Invoke-DevFlow ui resize $w $h
    if (-not $resize.Ok) { Add-Failure "Resize to ${w}x${h} failed: $($resize.Output)" }
    Start-Sleep -Seconds 2
    $script:proc.Refresh()
    if ($script:proc.MainWindowHandle -ne 0) {
        Write-Summary "- $theme-${size}: navigate=$($nav.Ok) tap=$($tap.Ok) resize=$($resize.Ok), window $(Get-WindowBounds $script:proc.MainWindowHandle)"
    }
}

function Save-LogError([string] $pageId, [string] $config) {
    $logs = Invoke-DevFlow logs --limit 60
    foreach ($line in ($logs.Output -split "`r?`n")) {
        if ($line -match '(?i)exception|binding.*(error|fail)|\bfail(ed|ure)?\b|\berror\b' -and $script:seenLogLines.Add($line)) {
            Add-Content $logErrors "[$config/$pageId] $line"
        }
    }
}
#endregion

function Save-PlatformShots {
    $selector = 'DotGridView ~ ContentView'
    $rawDir = Join-Path $OutDir 'platform-shots/raw/windows'
    New-Item -ItemType Directory -Force -Path $rawDir | Out-Null
    $shotIds = @(if ($Pages) { $Pages -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ } } else {
        Select-String -Path 'src/MauiGallery.Core/Catalog/PlatformShots.cs' -Pattern '^\s*"([a-z0-9-]+)",$' |
            ForEach-Object { $_.Matches[0].Groups[1].Value }
    })
    Write-Summary "## Platform shots ($($shotIds.Count) pages)`n"
    # A second pass retries pages that failed, for example when the app had to be relaunched.
    foreach ($pass in 1, 2) {
        foreach ($theme in 'light', 'dark') {
            $todo = @($shotIds | Where-Object { $pass -eq 1 -or -not (Test-Path (Join-Path $rawDir "$_-$theme.png")) })
            if ($todo.Count -eq 0) { continue }
            Set-Configuration $theme 'expanded'
            foreach ($id in $todo) {
                if (-not (Test-AppAlive)) {
                    Add-Failure "App was not running before $theme/$id; relaunching"
                    if (-not (Start-App)) { return }
                    Set-Configuration $theme 'expanded'
                }
                Invoke-DevFlow ui navigate "//main/controls/$id" | Out-Null
                $found = Invoke-DevFlow ui query --selector $selector --wait-until exists --timeout 15
                if (-not $found.Ok) { Add-Failure "No stage on $id ($theme)"; continue }
                Start-Sleep -Seconds $(if ($id -eq 'image') { 6 } else { $PageDelay })
                $query = Invoke-DevFlow ui query --selector $selector --fields 'id,bounds' --json
                try {
                    # The output starts with the CLI's "→ target: ... [agent ...]" line; the JSON array follows it.
                    if ($query.Output -notmatch '(?s)(?:^|\n)(\[\s*\{.*)$') { throw 'no JSON' }
                    $stage = @($Matches[1] | ConvertFrom-Json)[0]
                    if (-not $stage.id) { throw 'no element' }
                    } catch {
                    Add-Failure "No stage bounds for $id ($theme): $($query.Output)"
                    continue
                }
                # WinUI renders element screenshots without their alpha channel (transparent becomes black), so take
                # the window instead and let normalize.py crop the stage out of it: hide the dot grid, bring the stage
                # on screen and record where it is.
                $dots = Invoke-DevFlow ui query --selector DotGridView --fields id --json
                if ($dots.Output -match '(?s)(?:^|\n)(\[\s*\{.*)$') {
                    foreach ($dot in @($Matches[1] | ConvertFrom-Json)) { Invoke-DevFlow ui set-property $dot.id IsVisible False | Out-Null }
                }
                Invoke-DevFlow ui scroll --element $stage.id --position Center | Out-Null
                Start-Sleep -Seconds 1
                $element = Invoke-DevFlow ui element $stage.id
                try {
                    $window = ($element.Output.Substring($element.Output.IndexOf("`n{") + 1) | ConvertFrom-Json).windowBounds
                    @{ width = $stage.bounds.width; height = $stage.bounds.height; crop = $window } | ConvertTo-Json -Compress |
                        Set-Content (Join-Path $rawDir "$id-$theme.json")
                } catch {
                    Add-Failure "No window bounds for $id ($theme): $($element.Output)"
                    continue
                }
                $shot = Invoke-DevFlow ui screenshot --overwrite --output (Join-Path $rawDir "$id-$theme.png")
                if ($shot.Ok) { Write-Host "windows $theme $id" } else { Add-Failure "Screenshot failed for $id ($theme): $($shot.Output)" }
                Save-LogError $id "shots-$theme"
            }
        }
    }
}

function Stop-App {
    if ($script:proc -and -not $script:proc.HasExited) {
        Stop-Process -Id $script:proc.Id -Force -ErrorAction SilentlyContinue
        $script:proc.WaitForExit(10000) | Out-Null
    }
}

function Invoke-Repeat {
    $crashLog = Join-Path ([System.IO.Path]::GetTempPath()) 'MauiGallery-crash.log'
    $rows = [System.Collections.Generic.List[string]]::new()
    foreach ($variant in ($Variants -split ';' | ForEach-Object { $_.Trim() } | Where-Object { $_ })) {
        $script:appEnvironment = @{}
        if ($variant -ne 'default') {
            foreach ($pair in $variant -split ',') {
                $name, $value = $pair.Trim() -split '=', 2
                $script:appEnvironment[$name] = $value
            }
        }
        foreach ($id in $ids) {
            $visits = 0; $crashes = 0; $coldCrashes = 0
            Write-Summary "`n## Repeat $id ($variant)`n"
            for ($i = 0; $i -lt $Repeat; $i++) {
                $cold = $i % [Math]::Max($RelaunchEvery, 1) -eq 0
                if ($cold -or -not (Test-AppAlive)) {
                    Stop-App
                    if (-not (Start-App)) { Add-Failure "Could not launch the app ($variant)"; break }
                    Set-Configuration 'light' 'expanded'
                }
                foreach ($step in ($(if ($RepeatPath) { $RepeatPath } else { $RepeatFrom }) -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ })) {
                    switch ($step) {
                        'theme' {
                            $script:repeatTheme = if ($script:repeatTheme -eq 'dark') { 'light' } else { 'dark' }
                            Invoke-DevFlow ui navigate '//main/settings' | Out-Null
                            Start-Sleep -Seconds 1
                            Invoke-DevFlow ui tap "theme-$($script:repeatTheme)" | Out-Null
                        }
                        'resize' {
                            $script:repeatSize = if ($script:repeatSize -eq 'medium') { 'expanded' } else { 'medium' }
                            $w, $h = $sizes[$script:repeatSize]
                            Invoke-DevFlow ui resize $w $h | Out-Null
                        }
                        { $_ -in 'home', 'controls', 'settings' } { Invoke-DevFlow ui navigate "//main/$step" | Out-Null }
                        default { Invoke-DevFlow ui navigate "//main/controls/$step" | Out-Null }
                    }
                    Start-Sleep -Milliseconds 1500
                }
                $before = if (Test-Path $crashLog) { (Get-Item $crashLog).Length } else { 0 }
                Invoke-DevFlow ui navigate "//main/controls/$id" | Out-Null
                $stay = [Math]::Round((Get-Random -Minimum 0.3 -Maximum $RepeatDelay), 1)
                Start-Sleep -Seconds $stay
                # Leave again, possibly while the page is still loading, and give late callbacks time to fail.
                Invoke-DevFlow ui navigate "//main/controls/$RepeatFrom" | Out-Null
                Start-Sleep -Seconds 2
                $visits++
                if (-not (Test-AppAlive)) {
                    $crashes++
                    if ($cold) { $coldCrashes++ }
                    $what = ''
                    if ((Test-Path $crashLog) -and (Get-Item $crashLog).Length -gt $before) {
                        $stream = [System.IO.File]::OpenRead($crashLog)
                        try {
                            $stream.Seek($before, 'Begin') | Out-Null
                            $what = ([System.IO.StreamReader]::new($stream).ReadToEnd() -split "`r?`n" | Where-Object { $_ } | Select-Object -First 3) -join ' | '
                        } finally { $stream.Dispose() }
                    }
                    Add-Failure "CRASH on $id ($variant), visit $($i + 1)$(if ($cold) { ' (cold start)' }), stayed $stay s: $what"
                } else {
                    Save-LogError $id "repeat-$variant"
                }
            }
            $rows.Add("| $id | $variant | $visits | $crashes | $coldCrashes |")
        }
    }
    Stop-App
    Write-Summary "`n## Repeat results`n`n| Page | Variant | Visits | Crashes | On a cold start |`n| --- | --- | --- | --- | --- |"
    $rows | ForEach-Object { Write-Summary $_ }
}

# Page ids come from the catalog, so new pages are crawled without touching the workflow.
$ids = @(if ($Pages) {
    $Pages -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ }
} else {
    Get-ChildItem $CatalogDir -Filter 'GalleryCatalog.*.cs' |
        Select-String -Pattern '^\s*Id = "([^"]+)"' |
        ForEach-Object { $_.Matches[0].Groups[1].Value }
})
Write-Summary "Screen: $(Get-ScreenSize), pages: $($ids.Count), configurations: $Configs`n"

$script:seenLogLines = [System.Collections.Generic.HashSet[string]]::new()
Remove-Item (Join-Path ([System.IO.Path]::GetTempPath()) 'MauiGallery-*.log') -ErrorAction SilentlyContinue
if (-not (Start-App)) {
    Save-Screen (Join-Path $OutDir 'desktop/startup-failure.png')
    Write-Summary "`n## Failures`n"
    $failures | ForEach-Object { Write-Summary "- $_" }
    exit 0
}
Save-Screen (Join-Path $OutDir 'desktop/after-launch.png')

if ($Mode -eq 'PlatformShots') {
    Save-PlatformShots
    $Configs = ''
}
if ($Mode -eq 'Repeat') {
    Invoke-Repeat
    $Configs = ''
}

# Layout diagnostics need a newer agent than some builds ship; probe once and only use them if they work.
$probe = Invoke-DevFlow ui diagnostics --profile ci
Set-Content (Join-Path $OutDir 'diagnostics/probe.txt') $probe.Output
$diagnosticsSupported = $probe.Ok -and $probe.Output -notmatch '(?i)not (found|supported)|404|unknown'
Write-Summary "- Layout diagnostics supported: $diagnosticsSupported`n"

$firstConfig = $true
foreach ($config in ($Configs -split ',' | ForEach-Object { $_.Trim() } | Where-Object { $_ })) {
    $theme, $size = $config -split '-'
    Write-Summary "`n## $config"
    $pageDir = Join-Path $OutDir "pages/$config"
    New-Item -ItemType Directory -Force -Path $pageDir | Out-Null
    Set-Configuration $theme $size

    $windowShots = [ordered]@{}
    foreach ($routes in @($chromeRoutes) + @(if ($size -eq 'expanded') { $reviewRoutes })) {
        foreach ($key in $routes.Keys) { $windowShots[$key] = $routes[$key] }
    }
    foreach ($name in $windowShots.Keys) {
        Invoke-DevFlow ui navigate $windowShots[$name] | Out-Null
        Start-Sleep -Seconds $PageDelay
        if (-not (Test-AppAlive)) {
            Add-Failure "CRASH on $config/$name (window capture; exit code $($script:proc.ExitCode)); relaunching"
            if (-not (Start-App)) { break }
            Set-Configuration $theme $size
            continue
        }
        Save-WindowCapture (Join-Path $OutDir "review/$name-$theme-$($sizes[$size][0]).png") | Out-Null
        if ($name -eq 'settings') {
            # Native button sizes, to compare with what the window capture shows.
            (Invoke-DevFlow ui query --type Button --fields 'id,type,text,bounds').Output |
                Set-Content (Join-Path $OutDir "logs/settings-buttons-$config.txt")
        }
    }

    $methods = @{}
    foreach ($id in $ids) {
        if (-not (Test-AppAlive)) {
            Add-Failure "App was not running before $config/$id (exit code $($script:proc.ExitCode)); relaunching"
            if (-not (Start-App)) { break }
            Set-Configuration $theme $size
        }

        $nav = Invoke-DevFlow ui navigate "//main/controls/$id"
        if (-not $nav.Ok -or $nav.Output -match '"success"\s*:\s*false') {
            Add-Failure "Navigation to $id failed ($config): $($nav.Output -replace '\s+', ' ')"
        }
        Start-Sleep -Seconds $PageDelay

        if (-not (Test-AppAlive)) {
            Add-Failure "CRASH on $config/$id (exit code $($script:proc.ExitCode))"
            continue
        }

        # Web views start a browser process, which can take a while on a cold runner: capture them again later.
        if ($id -match 'web-view|^map$') {
            Start-Sleep -Seconds 6
            if (Test-AppAlive) { Save-WindowCapture (Join-Path $OutDir "review/$id-$theme-$($sizes[$size][0])-late.png") | Out-Null }
        }

        $method = Save-PageScreenshot (Join-Path $pageDir "$id.png")
        if ($method) { $methods[$method]++ } else { Add-Failure "No screenshot for $config/$id" }

        # Errors show up when a page is first created, so the first configuration is enough for per-page logs.
        if ($firstConfig) {
            Save-LogError $id $config
            if ($diagnosticsSupported) {
                (Invoke-DevFlow ui diagnostics --profile ci).Output | Set-Content (Join-Path $OutDir "diagnostics/$id.json")
            }
        }
    }
    $firstConfig = $false

    Write-Summary "- Screenshots: $(($methods.GetEnumerator() | ForEach-Object { "$($_.Key)=$($_.Value)" }) -join ', ')"
    if (Test-AppAlive) {
        (Invoke-DevFlow logs --limit 5000).Output | Set-Content (Join-Path $OutDir "logs/devflow-$config.txt")
    }
}

Save-Screen (Join-Path $OutDir 'desktop/end.png')

Get-WinEvent -FilterHashtable @{ LogName = 'Application'; StartTime = (Get-Date).AddHours(-3) } -ErrorAction SilentlyContinue |
    Where-Object { $_.ProviderName -match '\.NET Runtime|Application Error|Windows Error Reporting' } |
    Format-List TimeCreated, ProviderName, Id, Message |
    Out-File (Join-Path $OutDir 'logs/windows-events.txt')

if (Test-AppAlive) { Stop-Process -Id $script:proc.Id -Force }

# Debug builds write unhandled exceptions here (Platforms/Windows/App.xaml.cs).
$firstChanceLog = Join-Path ([System.IO.Path]::GetTempPath()) 'MauiGallery-first-chance.log'
if (Test-Path $firstChanceLog) { Copy-Item $firstChanceLog (Join-Path $OutDir 'logs/first-chance.log') }
$crashLog = Join-Path ([System.IO.Path]::GetTempPath()) 'MauiGallery-crash.log'
if (Test-Path $crashLog) {
    Copy-Item $crashLog (Join-Path $OutDir 'logs/crash.log')
    Add-Failure "Unhandled exceptions were logged: see logs/crash.log"
}

Write-Summary "`n## Failures ($($failures.Count))`n"
$failures | ForEach-Object { Write-Summary "- $_" }
Write-Summary "`nFinished $(Get-Date -Format o)"
exit 0
