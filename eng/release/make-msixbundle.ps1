# Bundles the per-architecture MSIX packages into one .msixbundle: the Microsoft Store takes a single upload per
# submission, and the Store Developer CLI publishes one file. The bundle stays unsigned; the Store signs it.
#
# Usage: eng/release/make-msixbundle.ps1 -PackagesDirectory <folder with *.msix> -Version <a.b.c.0> -Output <file.msixbundle>
#        [-Architectures x64,arm64]
param(
    [Parameter(Mandatory)] [string] $PackagesDirectory,
    [Parameter(Mandatory)] [string] $Version,
    [Parameter(Mandatory)] [string] $Output,
    [string[]] $Architectures = @('x64', 'arm64')
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression.FileSystem

$packages = @(Get-ChildItem -Path $PackagesDirectory -Filter '*.msix' -Recurse)
if ($packages.Count -eq 0) {
    throw "No .msix packages in $PackagesDirectory."
}

# Check that there's exactly one package per expected architecture, read from each package's manifest: a package
# built for the wrong architecture would otherwise end up in the bundle unnoticed.
$found = foreach ($package in $packages) {
    $zip = [System.IO.Compression.ZipFile]::OpenRead($package.FullName)
    try {
        $reader = [System.IO.StreamReader]::new($zip.GetEntry('AppxManifest.xml').Open())
        [xml]$manifest = $reader.ReadToEnd()
        $reader.Dispose()
    }
    finally {
        $zip.Dispose()
    }
    $architecture = $manifest.Package.Identity.ProcessorArchitecture
    Write-Host "$($package.Name): $architecture"
    $architecture
}
$actual = ($found | Sort-Object) -join ', '
$expected = ($Architectures | Sort-Object) -join ', '
if ($actual -ne $expected) {
    throw "Expected one package for each of $expected, found $actual."
}

# MakeAppx ships with the Windows SDK; take the newest one installed.
$makeAppx = Get-ChildItem -Path "${env:ProgramFiles(x86)}\Windows Kits\10\bin\*\x64\makeappx.exe" |
    Sort-Object { [version]$_.Directory.Parent.Name } -Descending |
    Select-Object -First 1
if (-not $makeAppx) {
    throw 'makeappx.exe not found: install the Windows SDK.'
}

# MakeAppx bundles every package in a folder, so copy them to an empty one.
$staging = Join-Path ([System.IO.Path]::GetTempPath()) "msixbundle-$([guid]::NewGuid())"
New-Item -ItemType Directory -Path $staging | Out-Null
try {
    foreach ($package in $packages) {
        Write-Host "Adding $($package.Name)"
        Copy-Item $package.FullName $staging
    }

    New-Item -ItemType Directory -Path (Split-Path -Parent $Output) -Force | Out-Null
    & $makeAppx.FullName bundle /d $staging /p $Output /bv $Version /o
    if ($LASTEXITCODE -ne 0) {
        throw "makeappx failed with exit code $LASTEXITCODE."
    }
}
finally {
    Remove-Item -Recurse -Force $staging
}

Write-Output "Created $Output ($Version) from $($packages.Count) packages."
