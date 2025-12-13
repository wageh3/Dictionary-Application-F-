module ErrorHandler

open System
open System.Windows.Forms
open Dictionary_Application.Models
open CRUD
open FileIO


let validateTerm (term: string) : Result<string, string> =
    let t = term.Trim()
    if String.IsNullOrWhiteSpace(t) then
        Error "Term cannot be empty."
    elif t.Contains("  ") then
        Error "Term contains invalid spacing."
    else 
        Ok t


let validateDefinition (definition: string) : Result<string, string> =
    let d = definition.Trim()
    if String.IsNullOrWhiteSpace(d) then
        Error "Definition cannot be empty."
    elif d.Length < 2 then
        Error "Definition is too short."
    else 
        Ok d


let safeAddWord (term: string) (definition: string) (dict: Map<string, Word>) : Map<string, Word> =
    match validateTerm term, validateDefinition definition with
    | Error e, _ -> 
        MessageBox.Show(e, "Invalid Term") |> ignore
        dict
    | _, Error e -> 
        MessageBox.Show(e, "Invalid Definition") |> ignore
        dict
    | Ok t, Ok d ->
        let key = t.ToLower()
        if dict.ContainsKey key then
            MessageBox.Show($"Word '{t}' already exists.", "Add Failed") |> ignore
            dict
        else
            CRUD.addWord t d dict


let safeUpdateWord (term: string) (definition: string) (dict: Map<string, Word>) : Map<string, Word> =
    match validateTerm term, validateDefinition definition with
    | Error e, _ -> 
        MessageBox.Show(e, "Invalid Term") |> ignore
        dict
    | _, Error e -> 
        MessageBox.Show(e, "Invalid Definition") |> ignore
        dict
    | Ok t, Ok d ->
        let key = t.ToLower()
        if dict.ContainsKey key then
            CRUD.updateWord t d dict
        else
            MessageBox.Show($"Word '{t}' not found for update.", "Update Failed") |> ignore
            dict


let safeDeleteWord (term: string) (dict: Map<string, Word>) : Map<string, Word> =
    match validateTerm term with
    | Error e -> 
        MessageBox.Show(e, "Invalid Term") |> ignore
        dict
    | Ok t ->
        let key = t.ToLower()
        if dict.ContainsKey key then
            CRUD.deleteWord t dict
        else
            MessageBox.Show($"Word '{t}' not found for deletion.", "Delete Failed") |> ignore
            dict


let safeSearchTerm (term: string) : string option =
    let t = term.Trim()
    if String.IsNullOrWhiteSpace(t) then None else Some(t.ToLower())


let handleInvalidChoice () =
    MessageBox.Show("Invalid choice, please try again.") |> ignore


let rec promptValidTerm () : string =
    let input = Console.ReadLine()
    match validateTerm input with
    | Ok t -> t
    | Error e -> 
        MessageBox.Show(e) |> ignore
        promptValidTerm ()

let rec promptValidDefinition () : string =
    let input = Console.ReadLine()
    match validateDefinition input with
    | Ok d -> d
    | Error e -> 
        MessageBox.Show(e) |> ignore
        promptValidDefinition ()