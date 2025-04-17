---
external help file: StartMenuManager.dll-Help.xml
Module Name: StartMenuManager
online version:
schema: 2.0.0
---

<!-- This Source Code Form is subject to the terms of the Mozilla Public
   - License, v. 2.0. If a copy of the MPL was not distributed with this
   - file, You can obtain one at https://mozilla.org/MPL/2.0/. -->

# Enable-StartMenu

## SYNOPSIS
Enables the Start Menu by renaming its directory back to the enabled state.

## SYNTAX

```
Enable-StartMenu [<CommonParameters>]
```

## DESCRIPTION
The `Enable-StartMenu` cmdlet enables the Start Menu by renaming the Start Menu directory back to its original state. This operation requires elevated privileges.

## EXAMPLES

### Example 1
```powershell
Enable-StartMenu
```

This command enables the Start Menu by renaming its directory back to the enabled state. If the Start Menu is already enabled, no action will be taken, and a message will be displayed.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### None

## NOTES
- This cmdlet requires administrative privileges to execute.
- If the Start Menu is already enabled, the cmdlet will output a message indicating no action was taken.

## RELATED LINKS
[Disable-StartMenu](./Disable-StartMenu.md)
[Get-StartMenuEnabled](./Get-StartMenuEnabled.md)
[Invoke-ToggleStartMenu](./Invoke-ToggleStartMenu.md)
