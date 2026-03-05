# Test script for version extraction logic
# Usage: .\test-version-extraction.ps1 v3.1.1.9

param(
    [Parameter(Mandatory=$true)]
    [string]$Tag
)

Write-Host "Testing version extraction logic" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

$version = $Tag -replace '^v', ''
Write-Host "Input Tag:         $Tag" -ForegroundColor Yellow
Write-Host "Extracted Version: $version" -ForegroundColor Yellow
Write-Host ""

# FileVersion: full version (e.g., 3.1.1.9)
$fileVersion = $version
Write-Host "FileVersion:       $fileVersion" -ForegroundColor Green

# PackageVersion: remove last segment (e.g., 3.1.1)
$parts = $version -split '\.'
if ($parts.Length -gt 3) {
    $packageVersion = ($parts[0..2] -join '.')
} else {
    $packageVersion = $version
}
Write-Host "PackageVersion:    $packageVersion" -ForegroundColor Green
Write-Host ""

Write-Host "Build Command Preview:" -ForegroundColor Cyan
Write-Host "dotnet pack WinFormsUI.Docking.sln ``" -ForegroundColor White
Write-Host "  --configuration Release ``" -ForegroundColor White
Write-Host "  -p:PackageVersion=$packageVersion ``" -ForegroundColor White
Write-Host "  -p:FileVersion=$fileVersion ``" -ForegroundColor White
Write-Host "  -p:AssemblyVersion=$fileVersion ``" -ForegroundColor White
Write-Host "  --output ./artifacts" -ForegroundColor White
Write-Host ""

Write-Host "Expected Packages:" -ForegroundColor Cyan
@(
    "DockPanelSuite.$packageVersion.nupkg",
    "DockPanelSuite.ThemeVS2003.$packageVersion.nupkg",
    "DockPanelSuite.ThemeVS2005.$packageVersion.nupkg",
    "DockPanelSuite.ThemeVS2005Multithreading.$packageVersion.nupkg",
    "DockPanelSuite.ThemeVS2012.$packageVersion.nupkg",
    "DockPanelSuite.ThemeVS2013.$packageVersion.nupkg",
    "DockPanelSuite.ThemeVS2015.$packageVersion.nupkg"
) | ForEach-Object { Write-Host "  - $_" -ForegroundColor White }
