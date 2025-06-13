# From https://janjones.me/posts/clickonce-installer-build-publish-github/.

[CmdletBinding(PositionalBinding=$false)]
param (
    [switch]$OnlyBuild=$false
)

$appName = "CameraThing" # 👈 Replace with your application project name.
$projDir = "CameraThing" # 👈 Replace with your project directory (where .csproj resides).

Set-StrictMode -version 2.0
$ErrorActionPreference = "Stop"

Write-Output "Working directory: $pwd"

# Load current Git tag.
$tag = $(git describe --tags 2>$null)
if (-not $tag) {
    Write-Output "No tags found, using default version"
    $tag = "v0.0.1"
}
Write-Output "Tag: $tag"

# Parse tag into a three-number version.
$version = $tag.Split('-')[0].TrimStart('v')
$version = "$version.0"
Write-Output "Version: $version"

# Clean output directory.
$publishDir = "bin/publish"
$outDir = "$projDir/$publishDir"
if (Test-Path $outDir) {
    Remove-Item -Path $outDir -Recurse
}

# Publish the application.
Push-Location $projDir
try {
    Write-Output "Restoring:"
    dotnet restore -r win-x64
    Write-Output "Publishing:"
    
    # Use dotnet publish instead of MSBuild for better .NET compatibility
    dotnet publish -c Release -r win-x64 --self-contained false `
        -p:PublishProfile=ClickOnceProfile `
        -p:ApplicationVersion=$version `
        -p:PublishDir=$publishDir `
        -p:PublishUrl=$publishDir

    # Measure publish size (with error handling).
    if (Test-Path "$publishDir/Application Files") {
        $publishSize = (Get-ChildItem -Path "$publishDir/Application Files" -Recurse |
            Measure-Object -Property Length -Sum).Sum / 1Mb
        Write-Output ("Published size: {0:N2} MB" -f $publishSize)
    } else {
        Write-Output "Warning: Application Files directory not found"
    }
}
finally {
    Pop-Location
}

if ($OnlyBuild) {
    Write-Output "Build finished."
    exit
}

# Clone `gh-pages` branch.
$ghPagesDir = "gh-pages"
if (-Not (Test-Path $ghPagesDir)) {
    git clone $(git config --get remote.origin.url) -b gh-pages `
        --depth 1 --single-branch $ghPagesDir
}

Push-Location $ghPagesDir
try {
    # Remove previous application files.
    Write-Output "Removing previous files..."
    if (Test-Path "Application Files") {
        Remove-Item -Path "Application Files" -Recurse
    }
    if (Test-Path "$appName.application") {
        Remove-Item -Path "$appName.application"
    }

    # Copy new application files.
    Write-Output "Copying new files..."
    Copy-Item -Path "../$outDir/Application Files","../$outDir/$appName.application" `
        -Destination . -Recurse

    # Stage and commit.
    Write-Output "Staging..."
    git add -A
    Write-Output "Committing..."
    git commit -m "Update to v$version"

    # Push.
    git push
} finally {
    Pop-Location
}