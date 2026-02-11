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
dotnet publish Speech.Google\Speech.Google.csproj -c Release -o Staging\Speech.Google --nologo -v q

# Build help XML from PlatyPS markdown → Staging/*/en-US/
Write-Host "`nBuilding help files..." -ForegroundColor Cyan
foreach ($mod in @('Speech.Core','Speech.Windows','Speech.Azure','Speech.OpenAI','Speech.Google')) {
    $helpScript = Join-Path $PSScriptRoot "$mod\PlatyPS\Build-Help.ps1"
    if (Test-Path $helpScript) { & $helpScript }
}
# Verify help XML in Staging
foreach ($mod in @('Speech.Core','Speech.Windows','Speech.Azure','Speech.OpenAI','Speech.Google')) {
    $xml = Join-Path $PSScriptRoot "Staging\$mod\en-US\$mod.dll-Help.xml"
    if (Test-Path $xml) { Write-Host "  [OK] $mod" -ForegroundColor Green }
    else { Write-Warning "  [MISSING] $mod en-US help XML" }
}
function Remove-And-Create {
    param([string]$Path)
    if (Test-Path $Path) {
        try { Remove-Item $Path -Recurse -Force -ErrorAction Stop }
        catch { Write-Warning "Could not clean $Path (files may be locked). Overwriting in place." }
    }
    New-Item -ItemType Directory -Path $Path -Force | Out-Null
}
# === Speech.Core ===
# NAudio is loaded here via RequiredAssemblies in psd1
# Other modules depend on Speech.Core, so NAudio is available to them
Write-Host "`nDeploying Speech.Core..." -ForegroundColor Yellow
$target = Join-Path $ModulePath 'Speech.Core'
Remove-And-Create $target

$coreFiles = @(
    'Speech.Core.dll', 'Speech.Core.psd1',
    'NAudio.dll', 'NAudio.Core.dll', 'NAudio.Wasapi.dll', 'NAudio.WinMM.dll'
)
foreach ($f in $coreFiles) {
    try { Copy-Item "Staging\Speech.Core\$f" $target -Force }
    catch { Write-Warning "  Skipped $f (locked)" }
}
# Help XML
New-Item -ItemType Directory -Path "$target\en-US" -Force | Out-Null; Copy-Item "Staging\Speech.Core\en-US\*" "$target\en-US" -Force

$count = (Get-ChildItem $target -Recurse -File).Count
Write-Host "  -> $count files" -ForegroundColor Green

# === Speech.Windows ===
Write-Host "Deploying Speech.Windows..." -ForegroundColor Yellow
$target = Join-Path $ModulePath 'Speech.Windows'
Remove-And-Create $target

foreach ($f in @('Speech.Windows.dll','Speech.Windows.psd1','Speech.Windows.format.ps1xml')) {
    try { Copy-Item "Staging\Speech.Windows\$f" $target -Force }
    catch { Write-Warning "  Skipped $f (locked)" }
}
# Help XML
New-Item -ItemType Directory -Path "$target\en-US" -Force | Out-Null; Copy-Item "Staging\Speech.Windows\en-US\*" "$target\en-US" -Force

$count = (Get-ChildItem $target -Recurse -File).Count
Write-Host "  -> $count files" -ForegroundColor Green

# === Speech.Azure ===
Write-Host "Deploying Speech.Azure..." -ForegroundColor Yellow
$target = Join-Path $ModulePath 'Speech.Azure'
Remove-And-Create $target

$azureFiles = @(
    'Speech.Azure.dll', 'Speech.Azure.psd1', 'Speech.Azure.format.ps1xml',
    'Microsoft.CognitiveServices.Speech.csharp.dll'
)
foreach ($f in $azureFiles) {
    try { Copy-Item "Staging\Speech.Azure\$f" $target -Force }
    catch { Write-Warning "  Skipped $f (locked)" }
}
# Windows x64 native libs
$nativeTarget = Join-Path $target 'runtimes\win-x64\native'
New-Item -ItemType Directory -Path $nativeTarget -Force | Out-Null
try { Copy-Item Staging\Speech.Azure\runtimes\win-x64\native\Microsoft.CognitiveServices.Speech*.dll $nativeTarget -Force }
catch { Write-Warning "  Skipped native DLLs (locked)" }
# Help XML
New-Item -ItemType Directory -Path "$target\en-US" -Force | Out-Null; Copy-Item "Staging\Speech.Azure\en-US\*" "$target\en-US" -Force

$count = (Get-ChildItem $target -Recurse -File).Count
Write-Host "  -> $count files" -ForegroundColor Green

# === Speech.OpenAI ===
Write-Host "Deploying Speech.OpenAI..." -ForegroundColor Yellow
$target = Join-Path $ModulePath 'Speech.OpenAI'
Remove-And-Create $target

foreach ($f in @('Speech.OpenAI.dll','Speech.OpenAI.psd1','Speech.OpenAI.format.ps1xml')) {
    try { Copy-Item "Staging\Speech.OpenAI\$f" $target -Force }
    catch { Write-Warning "  Skipped $f (locked)" }
}
# Help XML
New-Item -ItemType Directory -Path "$target\en-US" -Force | Out-Null; Copy-Item "Staging\Speech.OpenAI\en-US\*" "$target\en-US" -Force

$count = (Get-ChildItem $target -Recurse -File).Count
Write-Host "  -> $count files" -ForegroundColor Green

# === Speech.Google ===
Write-Host "Deploying Speech.Google..." -ForegroundColor Yellow
$target = Join-Path $ModulePath 'Speech.Google'
Remove-And-Create $target

foreach ($f in @('Speech.Google.dll','Speech.Google.psd1','Speech.Google.format.ps1xml')) {
    try { Copy-Item "Staging\Speech.Google\$f" $target -Force }
    catch { Write-Warning "  Skipped $f (locked)" }
}
# Help XML
New-Item -ItemType Directory -Path "$target\en-US" -Force | Out-Null; Copy-Item "Staging\Speech.Google\en-US\*" "$target\en-US" -Force

$count = (Get-ChildItem $target -Recurse -File).Count
Write-Host "  -> $count files" -ForegroundColor Green

# === Summary ===
Write-Host "`n=== Deployment Summary ===" -ForegroundColor Cyan
foreach ($mod in @('Speech.Core', 'Speech.Windows', 'Speech.Azure', 'Speech.OpenAI', 'Speech.Google')) {
    $path = Join-Path $ModulePath $mod
    $files = Get-ChildItem $path -Recurse -File
    $size = ($files | Measure-Object -Property Length -Sum).Sum / 1MB
    Write-Host ("{0,-20} {1,3} files, {2,6:N2} MB" -f $mod, $files.Count, $size)
}
