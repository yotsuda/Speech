# Speech Module PSGallery Publication Script
# Publishes all 6 modules in correct dependency order.
#
# Prerequisites:
#   1. Run Deploy.ps1 first to build and deploy all modules
#   2. Set $env:PSGALLERY_API_KEY or pass -ApiKey
#
# Usage:
#   .\Publish.ps1 "your-key"
#   .\Publish.ps1 -WhatIf            # Dry run: validate only, do not publish
#   .\Publish.ps1 -Module Speech.Core # Publish a single module

param(
    [Parameter(Position=0)]
    [string]$ApiKey = $env:PSGALLERY_API_KEY,
    [string]$ModulePath = 'C:\Program Files\PowerShell\7\Modules',
    [string[]]$Module,
    [switch]$WhatIf
)

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

# Publication order: Core first (no dependencies), then providers, then parent hub
$allModules = @(
    'Speech.Core',
    'Speech.Windows',
    'Speech.Azure',
    'Speech.OpenAI',
    'Speech.Google',
    'Speech'
)

# Filter to requested modules if specified
$modulesToPublish = if ($Module) {
    $Module | ForEach-Object {
        if ($_ -notin $allModules) { throw "Unknown module: $_. Valid: $($allModules -join ', ')" }
        $_
    }
} else {
    $allModules
}

# --- Validation ---
if (-not $WhatIf -and [string]::IsNullOrEmpty($ApiKey)) {
    throw "API key required. Set `$env:PSGALLERY_API_KEY or pass -ApiKey."
}

Write-Host "=== PSGallery Publication ===" -ForegroundColor Cyan
Write-Host "Source: $ModulePath" -ForegroundColor Gray
Write-Host "Modules: $($modulesToPublish -join ', ')" -ForegroundColor Gray
Write-Host ""

# --- Prepare publish directories (trimmed copies for size control) ---
$publishRoot = Join-Path $PSScriptRoot 'TempPublish'
if (Test-Path $publishRoot) { Remove-Item $publishRoot -Recurse -Force }

foreach ($mod in $modulesToPublish) {
    $src = Join-Path $ModulePath $mod
    if (-not (Test-Path $src)) {
        throw "Module directory not found: $src`nRun Deploy.ps1 first."
    }

    $dest = Join-Path $publishRoot $mod
    New-Item -ItemType Directory -Path $dest -Force | Out-Null

    if ($mod -eq 'Speech') {
        # Parent module: just the manifest
        Copy-Item "$src\Speech.psd1" $dest -Force
    }
    elseif ($mod -eq 'Speech.Azure') {
        # Azure: selective copy to control size
        $azureFiles = @(
            "$mod.dll", "$mod.psd1", "$mod.format.ps1xml",
            "$mod.deps.json", 'THIRD-PARTY-NOTICES.txt',
            'Microsoft.CognitiveServices.Speech.csharp.dll',
            'Azure.Core.dll', 'Microsoft.Bcl.AsyncInterfaces.dll',
            'System.ClientModel.dll', 'System.Memory.Data.dll',
            'Newtonsoft.Json.dll', 'Microsoft.ApplicationInsights.dll'
        )
        foreach ($f in $azureFiles) {
            $filePath = Join-Path $src $f
            if (Test-Path $filePath) { Copy-Item $filePath $dest -Force }
        }
        # Runtimes: only win-x64, win-arm64, and generic win (skip legacy win7/8/8.1/10)
        foreach ($rt in @('win-x64', 'win-arm64', 'win')) {
            $rtSrc = Join-Path $src "runtimes\$rt"
            if (Test-Path $rtSrc) {
                Copy-Item $rtSrc "$dest\runtimes\$rt" -Recurse -Force
            }
        }
        # Help XML
        $helpSrc = Join-Path $src 'en-US'
        if (Test-Path $helpSrc) {
            New-Item -ItemType Directory -Path "$dest\en-US" -Force | Out-Null
            Copy-Item "$helpSrc\*" "$dest\en-US" -Force
        }
    }
    else {
        # Core, Windows, OpenAI, Google: copy all files
        Get-ChildItem $src -File | Copy-Item -Destination $dest -Force
        # Help XML
        $helpSrc = Join-Path $src 'en-US'
        if (Test-Path $helpSrc) {
            New-Item -ItemType Directory -Path "$dest\en-US" -Force | Out-Null
            Copy-Item "$helpSrc\*" "$dest\en-US" -Force
        }
    }
}

# --- Validate manifests ---
Write-Host "Validating manifests..." -ForegroundColor Cyan
$hasError = $false
foreach ($mod in $modulesToPublish) {
    $psd1 = Get-ChildItem (Join-Path $publishRoot $mod) -Filter '*.psd1' | Select-Object -First 1
    try {
        $manifest = Test-ModuleManifest -Path $psd1.FullName -ErrorAction Stop
        $size = (Get-ChildItem (Join-Path $publishRoot $mod) -Recurse -File | Measure-Object -Property Length -Sum).Sum / 1MB
        Write-Host ("  [OK] {0,-20} v{1}  ({2:N1} MB)" -f $mod, $manifest.Version, $size) -ForegroundColor Green
    }
    catch {
        Write-Host "  [FAIL] $mod : $_" -ForegroundColor Red
        $hasError = $true
    }
}

if ($hasError) {
    throw "Manifest validation failed. Fix errors before publishing."
}

# --- Publish ---
if ($WhatIf) {
    Write-Host "`n[WhatIf] Validation passed. No modules were published." -ForegroundColor Yellow
}
else {
    Write-Host "`nPublishing to PSGallery..." -ForegroundColor Cyan
    foreach ($mod in $modulesToPublish) {
        $modPath = Join-Path $publishRoot $mod
        Write-Host "  Publishing $mod..." -ForegroundColor Yellow -NoNewline
        try {
            Publish-Module -Path $modPath -NuGetApiKey $ApiKey -Repository PSGallery -ErrorAction Stop
            Write-Host " Done" -ForegroundColor Green
        }
        catch {
            Write-Host " FAILED" -ForegroundColor Red
            throw "Failed to publish ${mod}: $_"
        }

        # Wait briefly between publishes for dependency resolution
        if ($mod -ne $modulesToPublish[-1]) {
            Write-Host "  Waiting 15s for PSGallery indexing..." -ForegroundColor Gray
            Start-Sleep -Seconds 15
        }
    }

    Write-Host "`n=== Publication Complete ===" -ForegroundColor Green
    Write-Host "Verify at: https://www.powershellgallery.com/profiles/yotsuda" -ForegroundColor Cyan
}

# --- Cleanup ---
if (Test-Path $publishRoot) {
    Remove-Item $publishRoot -Recurse -Force
}
