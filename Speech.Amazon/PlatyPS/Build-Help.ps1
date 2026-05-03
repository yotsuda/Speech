<#
.SYNOPSIS
    Build help files for Speech.Amazon module from PlatyPS markdown
.DESCRIPTION
    Generates MAML help from markdown documentation and copies to build directories.
.EXAMPLE
    .\Build-Help.ps1
.NOTES
    Requires: platyPS module (Install-Module -Name platyPS)
#>
[CmdletBinding()]
param()

$ErrorActionPreference = 'Stop'
$scriptRoot = $PSScriptRoot
$markdownPath = Join-Path $scriptRoot "en-US"
$projectRoot = Split-Path $scriptRoot -Parent
$binPath = Join-Path $projectRoot "bin"
$moduleName = "Speech.Amazon"

Write-Host "=== $moduleName Help Build ===" -ForegroundColor Cyan

if (-not (Get-Module -Name platyPS -ListAvailable)) {
    Write-Error "platyPS not found. Install: Install-Module -Name platyPS"
    exit 1
}
Import-Module platyPS -ErrorAction Stop

$mdFiles = Get-ChildItem $markdownPath -Filter "*.md"
Write-Host "[OK] $($mdFiles.Count) markdown files" -ForegroundColor Green

# Build to temp
$tempOut = Join-Path $env:TEMP "$moduleName-Help-Build"
if (Test-Path $tempOut) { Remove-Item $tempOut -Recurse -Force }

$result = New-ExternalHelp -Path $markdownPath -OutputPath $tempOut -Force -ErrorAction Continue
Write-Host "[OK] $($result.Name) ($([math]::Round($result.Length/1KB,1)) KB)" -ForegroundColor Green

# Copy to build directories
$helpFiles = Get-ChildItem $tempOut -Filter "*.xml"
foreach ($config in @("Debug","Release")) {
    $target = Join-Path $binPath "$config\net8.0\en-US"
    if (-not (Test-Path $target)) { New-Item $target -ItemType Directory -Force | Out-Null }
    foreach ($hf in $helpFiles) { Copy-Item $hf.FullName (Join-Path $target $hf.Name) -Force }
    Write-Host "  [COPY] $config/net8.0/en-US/" -ForegroundColor Gray
}

# Copy to Staging
$stagingTarget = "C:\MyProj\Speech\Staging\$moduleName\en-US"
if (-not (Test-Path $stagingTarget)) { New-Item $stagingTarget -ItemType Directory -Force | Out-Null }
foreach ($hf in $helpFiles) { Copy-Item $hf.FullName (Join-Path $stagingTarget $hf.Name) -Force }
Write-Host "  [COPY] Staging/$moduleName/en-US/" -ForegroundColor Gray

Remove-Item $tempOut -Recurse -Force -ErrorAction SilentlyContinue
Write-Host "=== Build Complete ===" -ForegroundColor Green
