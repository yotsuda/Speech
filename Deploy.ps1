# Speech Module Deployment Script
param(
    [string]$ModulePath = 'C:\Program Files\PowerShell\7\Modules'
)

$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

Write-Host "Building and publishing all modules..." -ForegroundColor Cyan
dotnet publish Speech.Core\Speech.Core.csproj -c Release -o Staging\Speech.Core --nologo -v q
dotnet publish Speech.Windows\Speech.Windows.csproj -c Release -o Staging\Speech.Windows --nologo -v q
dotnet publish Speech.Azure\Speech.Azure.csproj -c Release -o Staging\Speech.Azure --nologo -v q
dotnet publish Speech.OpenAI\Speech.OpenAI.csproj -c Release -o Staging\Speech.OpenAI --nologo -v q

function Remove-And-Create {
    param([string]$Path)
    if (Test-Path $Path) { Remove-Item $Path -Recurse -Force }
    New-Item -ItemType Directory -Path $Path -Force | Out-Null
}

# === Speech.Core ===
Write-Host "`nDeploying Speech.Core..." -ForegroundColor Yellow
$target = Join-Path $ModulePath 'Speech.Core'
Remove-And-Create $target

Copy-Item Staging\Speech.Core\Speech.Core.dll $target
Copy-Item Staging\Speech.Core\Speech.Core.psd1 $target
# NAudio for microphone access (only required DLLs)
Copy-Item Staging\Speech.Core\NAudio.Core.dll $target
Copy-Item Staging\Speech.Core\NAudio.Wasapi.dll $target
Copy-Item Staging\Speech.Core\NAudio.WinMM.dll $target

$count = (Get-ChildItem $target -File).Count
Write-Host "  -> $count files" -ForegroundColor Green

# === Speech.Windows ===
Write-Host "Deploying Speech.Windows..." -ForegroundColor Yellow
$target = Join-Path $ModulePath 'Speech.Windows'
Remove-And-Create $target

Copy-Item Staging\Speech.Windows\Speech.Windows.dll $target
Copy-Item Staging\Speech.Windows\Speech.Windows.psd1 $target

$count = (Get-ChildItem $target -File).Count
Write-Host "  -> $count files" -ForegroundColor Green

# === Speech.Azure ===
Write-Host "Deploying Speech.Azure..." -ForegroundColor Yellow
$target = Join-Path $ModulePath 'Speech.Azure'
Remove-And-Create $target

Copy-Item Staging\Speech.Azure\Speech.Azure.dll $target
Copy-Item Staging\Speech.Azure\Speech.Azure.psd1 $target
# Azure SDK managed DLL
Copy-Item Staging\Speech.Azure\Microsoft.CognitiveServices.Speech.csharp.dll $target
# Windows x64 native libs only
$nativeTarget = Join-Path $target 'runtimes\win-x64\native'
New-Item -ItemType Directory -Path $nativeTarget -Force | Out-Null
Copy-Item Staging\Speech.Azure\runtimes\win-x64\native\Microsoft.CognitiveServices.Speech*.dll $nativeTarget

$count = (Get-ChildItem $target -Recurse -File).Count
Write-Host "  -> $count files" -ForegroundColor Green

# === Speech.OpenAI ===
Write-Host "Deploying Speech.OpenAI..." -ForegroundColor Yellow
$target = Join-Path $ModulePath 'Speech.OpenAI'
Remove-And-Create $target

Copy-Item Staging\Speech.OpenAI\Speech.OpenAI.dll $target
Copy-Item Staging\Speech.OpenAI\Speech.OpenAI.psd1 $target

$count = (Get-ChildItem $target -File).Count
Write-Host "  -> $count files" -ForegroundColor Green

# === Summary ===
Write-Host "`n=== Deployment Summary ===" -ForegroundColor Cyan
foreach ($mod in @('Speech.Core', 'Speech.Windows', 'Speech.Azure', 'Speech.OpenAI')) {
    $path = Join-Path $ModulePath $mod
    $files = Get-ChildItem $path -Recurse -File
    $size = ($files | Measure-Object -Property Length -Sum).Sum / 1MB
    Write-Host ("{0,-20} {1,3} files, {2,6:N2} MB" -f $mod, $files.Count, $size)
}
