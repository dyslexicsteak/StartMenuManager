# This Source Code Form is subject to the terms of the Mozilla Public
# License, v. 2.0. If a copy of the MPL was not distributed with this
# file, You can obtain one at https://mozilla.org/MPL/2.0/.

@{
    # Script module or binary module file associated with this manifest.
    RootModule = 'StartMenuManager.dll'

    # Version number of this module.
    ModuleVersion = '0.1.0'

    # Supported PowerShell Editions
    CompatiblePSEditions = @('Core')

    # ID used to uniquely identify this module
    GUID = 'f987fcdf-0d79-4e9a-944e-9e1ec4229eb5'

    # Author of this module
    Author = 'dyslexicsteak'

    # Company or vendor of this module
    CompanyName = 'dyslexicsteak'

    # Copyright statement for this module
    Copyright = '(c) dyslexicsteak. All rights reserved.'

    # Description of the functionality provided by this module
    Description = 'A PowerShell module to manage the Windows Start Menu, including enabling, disabling, and toggling its state.'

    # Assemblies that must be loaded for the module to work
    RequiredAssemblies = @('FSharp.Core.dll')

    # Cmdlets to export from this module
    CmdletsToExport = @(
        'Invoke-ToggleStartMenu',
        'Enable-StartMenu',
        'Disable-StartMenu',
        'Get-StartMenuEnabled'
    )

    # Functions to export from this module
    FunctionsToExport = @()

    # Aliases to export from this module
    AliasesToExport = @()

    # DSC resources to export from this module
    DscResourcesToExport = @()

    # List of all files packaged with this module
    FileList = @(
        'StartMenuManager.dll',
        'FSharp.Core.dll',
        'StartMenuManager.psd1',
        'en-US\StartMenuManager.dll-help.xml',
        'en-US\about_StartMenuManagement.help.txt'
    )

    # Private data / metadata
    PrivateData = @{
        PSData = @{
            # Tags to help with module discovery in PowerShell Gallery
            Tags = @('StartMenu', 'Windows')

            # License URI for this module
            LicenseUri = 'https://www.mozilla.org/MPL/2.0/'

            # Project URI for this module
            ProjectUri = 'https://github.com/dyslexicsteak/StartMenuManager'

            # Release notes for this module
            ReleaseNotes = 'Initial release of StartMenuManager module with cmdlets to manage the Windows Start Menu.'
        }
    }
}
