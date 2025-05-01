// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace StartMenuManager

open System
open System.IO
open System.Diagnostics
open System.Management.Automation
open System.Security.Principal

module Cmdlets =
    [<Literal>]
    let SystemAppsFolderName = "SystemApps"

    [<Literal>]
    let MenuHostFolderName = "Microsoft.Windows.StartMenuExperienceHost_cw5n1h2txyewy"

    [<Literal>]
    let DisabledMenuHostFolderName = "disabled_Microsoft.Windows.StartMenuExperienceHost_cw5n1h2txyewy"

    let windowsFolderPath =
        Environment.GetFolderPath Environment.SpecialFolder.Windows

    let sysAppsPath =
        Path.Combine(windowsFolderPath, SystemAppsFolderName)

    let menuHostPath =
        Path.Combine(sysAppsPath, MenuHostFolderName)

    let disabledMenuHostPath =
        Path.Combine(sysAppsPath, DisabledMenuHostFolderName)

    [<Struct>]
    type RenameError =
        | Failed of Exception
        | Unauthorized of Exception
        | Unexpected of Exception

    [<Struct>]
    type TerminationError =
        | InvalidOp of id: int * ex: Exception
        | Unauthorized of id: int * ex: Exception
        | Unexpected of id: int * ex: Exception

    let isAdmin () =
        try
            WindowsIdentity.GetCurrent()
            |> WindowsPrincipal
            |> fun principal -> principal.IsInRole WindowsBuiltInRole.Administrator
        with
        | :? UnauthorizedAccessException -> false
        | _ -> false

    let ensureAdmin (cmdlet: Cmdlet) =
        if not (isAdmin ()) then
            cmdlet.ThrowTerminatingError(
                ErrorRecord(
                    UnauthorizedAccessException "This cmdlet requires elevated privileges.",
                    "NotAdmin",
                    ErrorCategory.PermissionDenied,
                    None
                )
            )

    let handleRenameError (cmdlet: Cmdlet) =
        function
        | Failed ex -> cmdlet.ThrowTerminatingError(ErrorRecord(ex, "RenameFailed", ErrorCategory.WriteError, None))
        | RenameError.Unauthorized ex ->
            cmdlet.ThrowTerminatingError(ErrorRecord(ex, "Unauthorized", ErrorCategory.PermissionDenied, None))
        | RenameError.Unexpected ex ->
            cmdlet.ThrowTerminatingError(ErrorRecord(ex, "Unexpected", ErrorCategory.NotSpecified, None))

    let handleTerminationError (cmdlet: Cmdlet) =
        function
        | InvalidOp(id, ex) ->
            cmdlet.ThrowTerminatingError(ErrorRecord(ex, "InvalidOp", ErrorCategory.InvalidOperation, id))
        | Unauthorized(id, ex) ->
            cmdlet.ThrowTerminatingError(ErrorRecord(ex, "Unauthorized", ErrorCategory.PermissionDenied, id))
        | Unexpected(id, ex) ->
            cmdlet.ThrowTerminatingError(ErrorRecord(ex, "Unexpected", ErrorCategory.NotSpecified, id))

    let terminateProcesses (procs: Process seq) =
        let killProcess (proc: Process) =
            use p = proc

            try
                p.Kill()

                if not (p.WaitForExit 5000) then
                    Error <| Unexpected(p.Id, TimeoutException "Termination timed out.")
                else
                    Ok()
            with
            | :? InvalidOperationException as ex -> Error <| InvalidOp(p.Id, ex)
            | :? UnauthorizedAccessException as ex -> Error <| Unauthorized(p.Id, ex)
            | ex -> Error <| Unexpected(p.Id, ex)

        procs
        |> Seq.tryPick (fun proc ->
            match killProcess proc with
            | Ok() -> None
            | Error err -> Some(Error err))
        |> Option.defaultValue (Ok())

    let moveDirectory (cmdlet: Cmdlet) source target =
        try
            Directory.Move(source, target)
            cmdlet.WriteVerbose $"Directory moved from {source} to {target} successfully."
            Ok()
        with
        | :? IOException as ex -> Error <| Failed ex
        | :? UnauthorizedAccessException as ex -> Error <| RenameError.Unauthorized ex
        | ex -> Error <| RenameError.Unexpected ex

    let enableMenu (cmdlet: Cmdlet) =
        cmdlet.WriteVerbose "Attempting to enable the Start Menu..."

        if Directory.Exists disabledMenuHostPath then
            if Directory.Exists menuHostPath then
                cmdlet.WriteVerbose "Start Menu is already enabled."
                Ok()
            else
                moveDirectory cmdlet disabledMenuHostPath menuHostPath
        else
            cmdlet.WriteVerbose "Disabled Start Menu directory does not exist."
            Error <| Failed(IOException "Target directory already exists.")

    let disableMenu (cmdlet: Cmdlet) =
        let rec attemptDisable () =
            let procs = Process.GetProcessesByName "StartMenuExperienceHost"
            cmdlet.WriteVerbose $"Found {procs.Length} StartMenuExperienceHost process(es)."

            match procs.Length, Directory.Exists menuHostPath with
            | n, true when n > 0 ->
                cmdlet.WriteVerbose "Terminating processes and renaming Start Menu directory..."
                terminateProcesses procs |> ignore
                moveDirectory cmdlet menuHostPath disabledMenuHostPath
            | _, true -> moveDirectory cmdlet menuHostPath disabledMenuHostPath
            | _ ->
                cmdlet.WriteVerbose "Start Menu directory does not exist. Nothing to disable."
                Ok()

        attemptDisable ()

    let isMenuEnabled () =
        try
            let isDirectoryPresent = Directory.Exists menuHostPath

            let isProcessRunning =
                Process.GetProcessesByName("StartMenuExperienceHost").Length > 0

            Ok(isDirectoryPresent && isProcessRunning)
        with
        | :? UnauthorizedAccessException as ex -> Error <| RenameError.Unauthorized ex
        | ex -> Error <| RenameError.Unexpected ex

    [<Cmdlet(VerbsLifecycle.Invoke, "ToggleStartMenu")>]
    type ToggleMenu() =
        inherit Cmdlet()

        override self.BeginProcessing() = ensureAdmin self

        override self.ProcessRecord() =
            self.WriteVerbose "Checking if Start Menu is enabled..."

            match isMenuEnabled () |> Result.defaultValue false with
            | true ->
                self.WriteVerbose "Start Menu is enabled. Disabling it..."
                disableMenu self |> ignore
            | false ->
                self.WriteVerbose "Start Menu is disabled. Enabling it..."
                enableMenu self |> ignore

    [<Cmdlet(VerbsLifecycle.Enable, "StartMenu")>]
    type EnableStartMenu() =
        inherit Cmdlet()

        override self.BeginProcessing() =
            ensureAdmin self

            if not (isMenuEnabled () |> Result.defaultValue false) then
                enableMenu self |> ignore
            else
                self.WriteVerbose "Start Menu is already enabled."

    [<Cmdlet(VerbsLifecycle.Disable, "StartMenu")>]
    type DisableStartMenu() =
        inherit Cmdlet()

        override self.BeginProcessing() =
            ensureAdmin self

            if isMenuEnabled () |> Result.defaultValue false then
                disableMenu self |> ignore
            else
                self.WriteVerbose "Start Menu is already disabled."

    [<Cmdlet(VerbsCommon.Get, "StartMenuEnabled")>]
    [<OutputType(typeof<bool>)>]
    type GetStartMenuEnabled() =
        inherit Cmdlet()

        override self.ProcessRecord() =
            self.WriteVerbose "Checking if Start Menu is enabled..."

            match isMenuEnabled () with
            | Ok status -> self.WriteObject status
            | Error err ->
                match err with
                | RenameError.Unauthorized ex ->
                    self.ThrowTerminatingError(ErrorRecord(ex, "Unauthorized", ErrorCategory.PermissionDenied, None))
                | RenameError.Unexpected ex ->
                    self.ThrowTerminatingError(ErrorRecord(ex, "Unexpected", ErrorCategory.NotSpecified, None))
                | Failed ex -> self.ThrowTerminatingError(ErrorRecord(ex, "Failed", ErrorCategory.NotSpecified, None))
