# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

param (
    [Parameter(Mandatory = $true)]
    [ValidateSet("Build", "Install")]
    [string]$Action,

    [Parameter(Mandatory = $true)]
    [string]$PublishDir,

    [string]$InstallPath,
    [string]$Configuration = "Release"
)

if ($VerbosePreference -ne "Continue") {
    $VerbosePreference = "Continue"
}

$outDir = $PublishDir
$moduleManifest = "$PsScriptRoot\StartMenuManager.psd1"
$moduleManifestOut = "$outDir\StartMenuManager.psd1"
$docsDir = "$PsScriptRoot\docs"
$markdownDir = Join-Path $docsDir "markdown"
$outputDocRoot = "$outDir\en-US"
$archivePath = "$outDir\StartMenuManager.zip"

function Build {
    Write-Verbose "Starting Build process..."

    # Ensure DLLs are present by running dotnet publish if missing
    $dllPath = Join-Path $outDir "StartMenuManager.dll"
    if (-not (Test-Path $dllPath)) {
        Write-Verbose "StartMenuManager.dll not found, running dotnet publish..."
        dotnet publish "$PsScriptRoot\StartMenuManager.fsproj" -c $Configuration -o $outDir
    }

    if (-not (Test-Path $outDir)) {
        Write-Verbose "Creating output directory at $outDir"
        New-Item -ItemType Directory -Path $outDir -Force | Out-Null
    }
    if (-not (Test-Path $outputDocRoot)) {
        Write-Verbose "Creating documentation output directory at $outputDocRoot"
        New-Item -ItemType Directory -Path $outputDocRoot -Force | Out-Null
    }

    Write-Verbose "Copying module manifest from $moduleManifest to $moduleManifestOut"
    Copy-Item -Path $moduleManifest -Destination $moduleManifestOut -Force

    # Copy about_*.help.txt files to en-US
    Write-Verbose "Copying about_*.help.txt files to $outputDocRoot"
    Get-ChildItem -Path $docsDir -Filter 'about_*.help.txt' | ForEach-Object {
        Copy-Item -Path $_.FullName -Destination $outputDocRoot -Force
    }

    if (Test-Path $markdownDir) {
        if (Get-Command -Name New-ExternalHelp -ErrorAction SilentlyContinue) {
            Write-Verbose "Generating external help from markdown directory $markdownDir"
            New-ExternalHelp -Path $markdownDir -OutputPath $outputDocRoot -Force
        }
        else {
            Write-Verbose "PlatyPS (New-ExternalHelp) is not installed. Only partial documentation will be available."
        }
    }
    else {
        Write-Warning "Markdown directory $markdownDir does not exist. Skipping external help generation."
    }

    Write-Verbose "Ensuring StartMenuManager.dll is present in $outDir"
    if (-not (Test-Path $dllPath)) {
        Write-Error "Missing DLL: $dllPath. Ensure the build process outputs the DLLs to $outDir."
        return
    }

    Write-Verbose "Compressing build output to $archivePath"
    if (Test-Path $archivePath) { Remove-Item $archivePath -Force }
    $itemsToZip = Get-ChildItem -Path $outDir
    Compress-Archive -Path $itemsToZip.FullName -DestinationPath $archivePath -Force -CompressionLevel Optimal
    Write-Verbose "Build process completed."

    # Remove all files and folders in $outDir except the zip file
    Write-Verbose "Cleaning up intermediate artefacts in $outDir"
    Get-ChildItem -Path $outDir | Where-Object { $_.Name -ne (Split-Path $archivePath -Leaf) } | Remove-Item -Recurse -Force
}

function Install {
    Write-Verbose "Starting Install process..."
    $dllPath = Join-Path $outDir "StartMenuManager.dll"
    if (-not (Test-Path $dllPath)) {
        Write-Verbose "StartMenuManager.dll not found, running dotnet publish..."
        dotnet publish "$PsScriptRoot\StartMenuManager.fsproj" -c $Configuration -o $outDir
    }
    if (-not (Test-Path $archivePath)) {
        Write-Verbose "Build output not found at $archivePath. Running Build first..."
        Build
    }

    if ($InstallPath) {
        $modulePath = $InstallPath
    }
    else {
        $modulePath = Join-Path ([Environment]::GetFolderPath([Environment+SpecialFolder]::MyDocuments)) "PowerShell\Modules\StartMenuManager"

    }

    if (-not (Test-Path $modulePath)) {
        Write-Verbose "Creating module directory at $modulePath"
        New-Item -ItemType Directory -Path $modulePath -Force | Out-Null
    }

    Write-Verbose "Expanding archive from $archivePath to $modulePath"
    Expand-Archive -Path $archivePath -DestinationPath $modulePath -Force
    Write-Verbose "Install process completed."
}

Write-Verbose "Action parameter received: $Action"
if ($Action -eq "Build") {
    Write-Verbose "Executing Build action..."
    Build
}
elseif ($Action -eq "Install") {
    Write-Verbose "Executing Install action..."
    Install
}
