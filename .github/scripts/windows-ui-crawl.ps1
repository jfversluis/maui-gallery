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
#>
param(
    [Parameter(Mandatory)] [string] $AppPath,
    [Parameter(Mandatory)] [string] $OutDir,
    [string] $CatalogDir = 'src/MauiGallery/Features/Catalog',
    # Comma-separated page ids; empty crawls the whole catalog.
    [string] $Pages = '',
    # Comma-separated <theme>-<size> configurations.
    [string] $Configs = 'light-expanded,dark-expanded,light-medium,dark-medium,light-compact,dark-compact',
    [double] $PageDelay = 2.0
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

function Invoke-DevFlow {
    param([Parameter(ValueFromRemainingArguments)] [string[]] $Arguments)
    $all = @('devflow') + $Arguments
    if ($script:port) { $all += @('--agent-port', $script:port) }
    $output = & maui @all 2>&1 | Out-String
    [pscustomobject]@{ Ok = ($LASTEXITCODE -eq 0); Output = $output.Trim() }
}

function Start-App {
    Write-Host "Launching $AppPath"
    $script:proc = Start-Process -FilePath $AppPath -WorkingDirectory (Split-Path $AppPath) -PassThru
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
Remove-Item (Join-Path ([System.IO.Path]::GetTempPath()) 'MauiGallery-crash.log') -ErrorAction SilentlyContinue
if (-not (Start-App)) {
    Save-Screen (Join-Path $OutDir 'desktop/startup-failure.png')
    Write-Summary "`n## Failures`n"
    $failures | ForEach-Object { Write-Summary "- $_" }
    exit 0
}
Save-Screen (Join-Path $OutDir 'desktop/after-launch.png')

# Layout diagnostics need a newer agent than some builds ship; probe once and only use them if they work.
$probe = Invoke-DevFlow ui diagnostics --profile ci
Set-Content (Join-Path $OutDir 'diagnostics/probe.txt') $probe.Output
$diagnosticsSupported = $probe.Ok -and $probe.Output -notmatch '(?i)not (found|supported)|404|unknown'
Write-Summary "- Layout diagnostics supported: $diagnosticsSupported`n"

$firstConfig = $true
foreach ($config in ($Configs -split ',' | ForEach-Object { $_.Trim() })) {
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
        Save-WindowCapture (Join-Path $OutDir "review/$name-$theme-$($sizes[$size][0]).png") | Out-Null
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
$crashLog = Join-Path ([System.IO.Path]::GetTempPath()) 'MauiGallery-crash.log'
if (Test-Path $crashLog) {
    Copy-Item $crashLog (Join-Path $OutDir 'logs/crash.log')
    Add-Failure "Unhandled exceptions were logged: see logs/crash.log"
}

Write-Summary "`n## Failures ($($failures.Count))`n"
$failures | ForEach-Object { Write-Summary "- $_" }
Write-Summary "`nFinished $(Get-Date -Format o)"
exit 0
