---
external help file: StartMenuManager.dll-Help.xml
Module Name: StartMenuManager
online version:
schema: 2.0.0
---

<!-- This Source Code Form is subject to the terms of the Mozilla Public
   - License, v. 2.0. If a copy of the MPL was not distributed with this
   - file, You can obtain one at https://mozilla.org/MPL/2.0/. -->

# Get-StartMenuEnabled

## SYNOPSIS
Checks if the Start Menu is currently enabled.

## SYNTAX

```
Get-StartMenuEnabled [<CommonParameters>]
```

## DESCRIPTION
The `Get-StartMenuEnabled` cmdlet checks if the Start Menu is currently enabled by verifying the existence of the Start Menu directory and whether the `StartMenuExperienceHost` process is running.

## EXAMPLES

### Example 1
```powershell
Get-StartMenuEnabled
True
```

This command checks if the Start Menu is enabled. It returns `True` if the Start Menu is enabled and `False` otherwise.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### System.Boolean
Returns `True` if the Start Menu is enabled, otherwise `False`.

## NOTES
- This cmdlet does not require administrative privileges.

## RELATED LINKS
[Enable-StartMenu](./Enable-StartMenu.md)
[Disable-StartMenu](./Disable-StartMenu.md)
[Invoke-ToggleStartMenu](./InvokeToggleStartMenu.md)
