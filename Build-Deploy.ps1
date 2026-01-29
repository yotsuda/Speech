<#
.SYNOPSIS
    Build and deploy Speech module to PowerShell 7 Modules directory.

.DESCRIPTION
    Builds the Speech module in Release configuration and deploys to:
    C:\Program Files\PowerShell\7\Modules\Speech

.PARAMETER BuildOnly
    Only build, do not deploy.

.PARAMETER DeployOnly
    Only deploy (skip build). Requires previous build.

.EXAMPLE
    .\Build-Deploy.ps1
    # Build and deploy

.EXAMPLE
    .\Build-Deploy.ps1 -BuildOnly
    # Build only
#>
[CmdletBinding()]
param(
    [switch]$BuildOnly,
    [switch]$DeployOnly
)

$ErrorActionPreference = 'Stop'

$ProjectRoot = $PSScriptRoot
$ProjectFile = Join-Path $ProjectRoot 'Speech\Speech.csproj'
$BuildDir = Join-Path $ProjectRoot 'Speech\bin\Release\net9.0'
$StagingDir = Join-Path $ProjectRoot 'Staging'
$DeployDir = 'C:\Program Files\PowerShell\7\Modules\Speech'

# =============================================================================
# Build
# =============================================================================
if (-not $DeployOnly) {
    Write-Host '========================================' -ForegroundColor Cyan
    Write-Host ' Building Speech module...' -ForegroundColor Cyan
    Write-Host '========================================' -ForegroundColor Cyan

    dotnet publish $ProjectFile -c Release -o $BuildDir

    if ($LASTEXITCODE -ne 0) {
        Write-Error 'Build failed.'
        exit 1
    }

    Write-Host 'Build succeeded.' -ForegroundColor Green
    Write-Host ''
}

if ($BuildOnly) {
    Write-Host "Output: $BuildDir"
    exit 0
}

# =============================================================================
# Deploy
# =============================================================================
Write-Host '========================================' -ForegroundColor Cyan
Write-Host ' Deploying Speech module...' -ForegroundColor Cyan
Write-Host '========================================' -ForegroundColor Cyan

# Check if build directory exists
if (-not (Test-Path $BuildDir)) {
    Write-Error "Build directory not found: $BuildDir. Run build first."
    exit 1
}

# Remove existing deployment
if (Test-Path $DeployDir) {
    Write-Host "Removing existing deployment: $DeployDir"
    Remove-Item $DeployDir -Recurse -Force
}

# Create deployment directory
New-Item -ItemType Directory -Path $DeployDir -Force | Out-Null

# DLL files to copy from BuildDir
$dllFiles = @(
    'Speech.dll',
    'Speech.deps.json',
    'Microsoft.CognitiveServices.Speech.csharp.dll',
    'NAudio.dll',
    'NAudio.Core.dll',
    'NAudio.Asio.dll',
    'NAudio.Midi.dll',
    'NAudio.Wasapi.dll',
    'NAudio.WinMM.dll',
    'System.Speech.dll',
    'Azure.Core.dll',
    'System.ClientModel.dll',
    'System.Memory.Data.dll',
    'Microsoft.Bcl.AsyncInterfaces.dll',
    'Newtonsoft.Json.dll'
)

# Module definition files to copy from StagingDir
$moduleFiles = @(
    'Speech.psd1',
    'Speech.psm1',
    'Speech.Format.ps1xml'
)

Write-Host 'Copying DLL files from build output...'
foreach ($file in $dllFiles) {
    $src = Join-Path $BuildDir $file
    if (Test-Path $src) {
        Copy-Item $src -Destination $DeployDir
        Write-Host "  $file" -ForegroundColor Gray
    }
}

Write-Host 'Copying module definition files from Staging...'
foreach ($file in $moduleFiles) {
    $src = Join-Path $StagingDir $file
    if (Test-Path $src) {
        Copy-Item $src -Destination $DeployDir
        Write-Host "  $file" -ForegroundColor Gray
    } else {
        Write-Warning "Module file not found: $src"
    }
}

# Copy win-x64 native runtimes (required for Azure Speech SDK)
$nativeSource = Join-Path $BuildDir 'runtimes\win-x64\native'
$nativeDest = Join-Path $DeployDir 'runtimes\win-x64\native'

if (Test-Path $nativeSource) {
    Write-Host 'Copying native runtimes (win-x64)...'
    New-Item -ItemType Directory -Path $nativeDest -Force | Out-Null
    Copy-Item "$nativeSource\*" -Destination $nativeDest -Recurse
    Get-ChildItem $nativeDest -File | ForEach-Object {
        Write-Host "  runtimes\win-x64\native\$($_.Name)" -ForegroundColor Gray
    }

    # Also copy Azure Speech SDK native DLLs to module root (required for proper loading)
    Write-Host 'Copying Azure Speech SDK native DLLs to module root...'
    Get-ChildItem "$nativeSource\Microsoft.CognitiveServices.Speech.*" -ErrorAction SilentlyContinue | ForEach-Object {
        Copy-Item $_.FullName -Destination $DeployDir
        Write-Host "  $($_.Name)" -ForegroundColor Gray
    }
}

Write-Host ''
Write-Host '========================================' -ForegroundColor Green
Write-Host ' Deployment complete!' -ForegroundColor Green
Write-Host '========================================' -ForegroundColor Green
Write-Host ''
Write-Host "Deployed to: $DeployDir" -ForegroundColor Yellow
Write-Host ''
Write-Host 'To verify:' -ForegroundColor Cyan
Write-Host '  Get-Module Speech -ListAvailable'
Write-Host '  Import-Module Speech -Force'
Write-Host '  Get-Command -Module Speech'
