# StartMenuManager

StartMenuManager is a PowerShell module for managing the Windows Start Menu written in F#. It provides cmdlets to enable, disable, toggle, and query the state of the Start Menu.

## Table of Contents

- [StartMenuManager](#startmenumanager)
  - [Table of Contents](#table-of-contents)
  - [Features](#features)
  - [Cmdlet Overview](#cmdlet-overview)
    - [Details](#details)
  - [Build and Install Instructions](#build-and-install-instructions)
    - [Requirements](#requirements)
      - [Build Requirements](#build-requirements)
      - [Usage Requirements](#usage-requirements)
    - [Quick Start (Recommended)](#quick-start-recommended)
      - [Build Only (default)](#build-only-default)
      - [Build and Install (in one step)](#build-and-install-in-one-step)
    - [Advanced: Manual Build/Install](#advanced-manual-buildinstall)
      - [build.ps1 Parameters](#buildps1-parameters)
  - [Defaults and How to Change Them](#defaults-and-how-to-change-them)
  - [What Happens During Build and Install](#what-happens-during-build-and-install)
  - [Using the Module](#using-the-module)
  - [Documentation](#documentation)
  - [Troubleshooting](#troubleshooting)
  - [License](#license)

## Features

- **Enable-StartMenu**: Restores the Start Menu if it has been disabled.
- **Disable-StartMenu**: Disables the Start Menu by renaming its app directory.
- **Invoke-ToggleStartMenu**: Toggles the Start Menu between enabled and disabled states.
- **Get-StartMenuEnabled**: Checks if the Start Menu is currently enabled.

## Cmdlet Overview

| Cmdlet                  | Requires Admin | Description                                                                                   | How it Works                                                                                      |
|-------------------------|:--------------:|-----------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------|
| `Enable-StartMenu`      |      Yes       | Enables the Start Menu if disabled.                                                           | Renames the Start Menu app directory back to its original name.                                   |
| `Disable-StartMenu`     |      Yes       | Disables the Start Menu.                                                                      | Terminates the Start Menu process and renames its app directory to prevent it from restarting.    |
| `Invoke-ToggleStartMenu`|      Yes       | Toggles the Start Menu between enabled and disabled states.                                   | Checks current state and calls Enable or Disable as appropriate.                                  |
| `Get-StartMenuEnabled`  |      No        | Checks if the Start Menu is currently enabled.                                                | Verifies if the Start Menu directory exists and if the Start Menu process is running.             |

### Details

- **Enable-StartMenu**
  *Requires administrative privileges.*
  If the Start Menu app directory has been renamed (disabled), this cmdlet renames it back to its original name. This allows Windows to restart the Start Menu process automatically.

- **Disable-StartMenu**
  *Requires administrative privileges.*
  This cmdlet attempts to terminate the `StartMenuExperienceHost` process and then renames its app directory. Renaming prevents Windows from restarting the process, effectively disabling the Start Menu.

- **Invoke-ToggleStartMenu**
  *Requires administrative privileges.*
  Checks if the Start Menu is enabled. If so, disables it; if disabled, enables it. Useful for quickly switching states.

- **Get-StartMenuEnabled**
  *Does not require administrative privileges.*
  Returns `True` if the Start Menu is enabled (directory exists and process is running), otherwise `False`.

> **Note:**
> Disabling or enabling the Start Menu involves manipulating system files and processes. These operations require administrator rights and may briefly disrupt the desktop environment.

## Build and Install Instructions

### Requirements

#### Build Requirements

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- PowerShell 5.1+ or PowerShell Core
- PlatyPS (optional, to build documentation)

#### Usage Requirements

- PowerShell 5.1+ or PowerShell Core
- Windows OS

### Quick Start (Recommended)

The easiest way to build and install is with `dotnet publish`, which passes MSBuild properties through to the build system and triggers all packaging and optional install steps.

#### Build Only (default)

```sh
dotnet publish
```

- **Default output directory:** `/bin/Release/net9.0/publish/`
- **Default configuration:** `Release`

#### Build and Install (in one step)

To build and install the module to your PowerShell modules folder (or a custom path), use the `EnableInstall` property:

```sh
dotnet publish -p:EnableInstall=true
```

- **Default install path:** If not specified, the install path is:
  `"MyDocuments/PowerShell/Modules/StartMenuManager/"` (for the current user).
  Using MyDocuments as reported by
  `[Environment]::GetFolderPath([Environment+SpecialFolder]::MyDocuments)`, you
  can execute this in PowerShell to identify this path for your user.

To specify a custom install path:

```sh
dotnet publish -p:EnableInstall=true -p:InstallPath="C:/Some/Other/Path/"
```

Note that when using a custom install path `StartMenuManager/` is not appended, meaning the files will be placed directly in the specified path.

---

### Advanced: Manual Build/Install

You can simply run a normal build to get the library on its own:

```sh
dotnet build
```

This does not build documentation or any dependencies.

You can also invoke the build script directly bypassing MSBuild:

#### build.ps1 Parameters

| Parameter        | Required | Description                                                                                   | Example Value                                               |
|------------------|----------|-----------------------------------------------------------------------------------------------|-------------------------------------------------------------|
| `-Action`        | Yes      | The action to perform. Must be either `Build` or `Install`.                                   | `Build` or `Install`                                        |
| `-PublishDir`    | Yes      | The directory where the build output (DLLs, manifest, docs) will be placed.                   | `StartMenuManager/bin/Release/net9.0/publish`               |
| `-InstallPath`   | No       | The directory to install the module to (used only with `-Action Install`).                    | `"$HOME/Documents/PowerShell/Modules/StartMenuManager"`     |
| `-Configuration` | No       | The build configuration to use. Defaults to `Release`.                                        | `Release` or `Debug`                                        |

**Example usage:**

```sh
pwsh -NoProfile -ExecutionPolicy Bypass -File StartMenuManager/build.ps1 -Action Build -PublishDir StartMenuManager/bin/Release/net9.0/publish -Configuration Release
```

```sh
pwsh -NoProfile -ExecutionPolicy Bypass -File StartMenuManager/build.ps1 -Action Build -PublishDir StartMenuManager/bin/Release/net9.0/publish -Configuration Release
```

To install:

```sh
pwsh -NoProfile -ExecutionPolicy Bypass -File StartMenuManager/build.ps1 -Action Install -PublishDir StartMenuManager/bin/Release/net9.0/publish -Configuration Release -InstallPath "$HOME/Documents/PowerShell/Modules/StartMenuManager"
```

## Defaults and How to Change Them

| Property         | Default Value                                                                 | How to Change                                                                                      |
|------------------|-------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------|
| `Configuration`  | `Release`                                                                     | Pass `-c Debug` or `-c Release` to `dotnet publish` or `-Configuration Debug` to the build script. |
| `PublishDir`     | `bin/<Configuration>/net9.0/publish`                                          | Pass `-o <dir>` to `dotnet publish` or `-PublishDir <dir>` to the build script.                    |
| `EnableInstall`  | `false`                                                                       | Pass `-p:EnableInstall=true` to `dotnet publish` or `/p:EnableInstall=true` to MSBuild.            |
| `InstallPath`    | `"$HOME/Documents/PowerShell/Modules/StartMenuManager"` (current user module) | Pass `-p:InstallPath="C:/Custom/Path"` to `dotnet publish` or `-InstallPath <dir>` to the script.  |
| `VerboseFlag`    | Not set (no verbose output)                                                   | Set MSBuild property `Verbosity=diag` or pass `-Verbosity diag` to `dotnet publish`.               |

- All MSBuild properties can be set via `-p:<Name>=<Value>` on the `dotnet publish` command line.

## What Happens During Build and Install

1. **dotnet publish** builds the F# project and outputs all binaries to the publish directory.
2. **FinalizePublish** MSBuild target (triggered automatically after publish):
    - Runs `build.ps1` with `-Action Build` to:
        - Copy the module manifest and documentation.
        - Generate external help from markdown.
        - Package the output as a zip.
        - Clean up intermediate files.
    - If `EnableInstall=true`, runs `build.ps1` with `-Action Install` to:
        - Extract the packaged module to the install path.

## Using the Module

After installation, import the module in PowerShell:

```powershell
Import-Module StartMenuManager
```

If you installed the module somewhere in your `env:PSModulePath` then it will automatically be loaded by PowerShell.

You can now use the cmdlets:

```powershell
Enable-StartMenu
Disable-StartMenu
Invoke-ToggleStartMenu
Get-StartMenuEnabled
```

## Documentation

- See the `docs/` folder for markdown help and about topics.
- External help is generated automatically during the build and placed in `en-US/` in the output directory.

## Troubleshooting

- If install fails, check that you have write permissions to the install path.
- For verbose output, use `-v diag`, `--verbosity diag`, or `-p:Verbosity=diag` with `dotnet publish`.

## License

MPL 2.0 License, see [License Text](https://www.mozilla.org/en-US/MPL/2.0/) or LICENSE.txt for more details.
