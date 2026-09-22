param(
    [string]$ValheimPath = "D:\SteamLibrary\steamapps\common\Valheim",
    [string]$DeployProfile = "C:\Users\cdjen\AppData\Roaming\com.kesomannen.gale\valheim\profiles\New Release",
    [switch]$Deploy,
    [switch]$Package
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$project = Join-Path $root "src\DaylightSavings\DaylightSavings.csproj"
$dll = Join-Path $root "artifacts\DaylightSavings.dll"
$thunderstore = Join-Path $root "thunderstore"
$manifest = Get-Content (Join-Path $thunderstore "manifest.json") | ConvertFrom-Json

dotnet build $project "-p:ValheimPath=$ValheimPath" -c Release
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Built: $dll"

if ($Deploy) {
    $pluginDir = Join-Path $DeployProfile "BepInEx\plugins\Hardwire99-DaylightSavings"
    New-Item -ItemType Directory -Force -Path $pluginDir | Out-Null
    $dest = Join-Path $pluginDir "DaylightSavings.dll"

    try {
        Copy-Item $dll $dest -Force
        Copy-Item (Join-Path $thunderstore "manifest.json") (Join-Path $pluginDir "manifest.json") -Force
        Copy-Item (Join-Path $thunderstore "CHANGELOG.md") (Join-Path $pluginDir "CHANGELOG.md") -Force
        Write-Host "Deployed to $dest"
    }
    catch {
        $pending = Join-Path $pluginDir "DaylightSavings.dll.pending"
        Copy-Item $dll $pending -Force
        Write-Warning "Valheim has the plugin locked. Close the game, then replace DaylightSavings.dll with DaylightSavings.dll.pending"
        Write-Host "Built update saved to $pending"
    }
}

if ($Package) {
    $staging = Join-Path $root "artifacts\thunderstore-staging"
    $team = "Hardwire99"
    $packageName = "{0}-{1}-{2}.zip" -f $team, $manifest.name, $manifest.version_number
    $packagePath = Join-Path (Join-Path $root "artifacts") $packageName
    $iconSource = Join-Path $thunderstore "icon.png"

    if (-not (Test-Path $iconSource)) {
        throw "thunderstore\icon.png is missing. Thunderstore requires a 256x256 PNG."
    }

    if (Test-Path $staging) {
        Remove-Item $staging -Recurse -Force
    }
    New-Item -ItemType Directory -Force -Path $staging | Out-Null

    Copy-Item $dll (Join-Path $staging "DaylightSavings.dll") -Force
    Copy-Item (Join-Path $thunderstore "manifest.json") (Join-Path $staging "manifest.json") -Force
    Copy-Item (Join-Path $thunderstore "README.md") (Join-Path $staging "README.md") -Force
    Copy-Item (Join-Path $thunderstore "CHANGELOG.md") (Join-Path $staging "CHANGELOG.md") -Force
    Copy-Item $iconSource (Join-Path $staging "icon.png") -Force

    if (Test-Path $packagePath) {
        Remove-Item $packagePath -Force
    }

    Compress-Archive -Path (Join-Path $staging "*") -DestinationPath $packagePath -Force
    Write-Host "Packaged: $packagePath"
    Write-Host "Upload as team $team. Thunderstore will show the name as '$($manifest.name)'."
}

if (-not $Deploy -and -not $Package) {
    Write-Host "Skipped deploy/package. Pass -Deploy and/or -Package as needed."
}
