---
external help file: StartMenuManager.dll-Help.xml
Module Name: StartMenuManager
online version:
schema: 2.0.0
---

<!-- This Source Code Form is subject to the terms of the Mozilla Public
   - License, v. 2.0. If a copy of the MPL was not distributed with this
   - file, You can obtain one at https://mozilla.org/MPL/2.0/. -->

# Invoke-ToggleStartMenu

## SYNOPSIS
Toggles the Start Menu between enabled and disabled states.

## SYNTAX

```
Invoke-ToggleStartMenu [<CommonParameters>]
```

## DESCRIPTION
The `Invoke-ToggleStartMenu` cmdlet toggles the Start Menu between enabled and disabled states. If the Start Menu is currently enabled, it will be disabled, and vice versa. This operation requires elevated privileges.

## EXAMPLES

### Example 1
```powershell
Invoke-ToggleStartMenu
```

This command toggles the Start Menu state. If the Start Menu is currently enabled, it will be disabled. If it is disabled, it will be enabled. This operation requires elevated privileges.

## PARAMETERS

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None

## OUTPUTS

### None

## NOTES
- This cmdlet requires administrative privileges to execute.

## RELATED LINKS
[Enable-StartMenu](./EnableStartMenu.md)
[Disable-StartMenu](./Disable-StartMenu.md)
[Get-StartMenuEnabled](./Get-StartMenuEnabled.md)
