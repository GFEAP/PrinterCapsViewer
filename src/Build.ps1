$ErrorActionPreference = "Stop"

Write-Host "Restore..."
msbuild PrinterCapsViewer.sln -Restore

Write-Host "Build..."
msbuild Application\Application.csproj `
    /p:Configuration=Release

Write-Host "Publish..."
dotnet publish Application\Application.csproj `
    -c Release `
    -f net8.0-windows `
    -r win-x64 `
    --self-contained false

Write-Host "Build MSI..."
msbuild Installer\PrinterCapsViewerInstaller.wixproj `
    -Restore `
    /p:Configuration=Release

Write-Host "Fertig."
