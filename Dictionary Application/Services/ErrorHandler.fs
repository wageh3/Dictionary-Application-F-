module ErrorHandler

open System
open Dictionary_Application.Models
open CRUD
open FileIO


let validateTerm (term: string) : Result<string, string> =
    let t = term.Trim()
    if String.IsNullOrWhiteSpace(t) then
        Error "Term cannot be empty."
    elif t.Contains("  ") then
        Error "Term contains invalid spacing."
    else Ok t

let validateDefinition (definition: string) : Result<string, string> =
    let d = definition.Trim()
    if String.IsNullOrWhiteSpace(d) then
        Error "Definition cannot be empty."
    elif d.Length < 2 then
        Error "Definition is too short."
    else Ok d


let safeAddWord (term: string) (definition: string) (dict: Map<string, Word>) : Map<string, Word> =
    match validateTerm term, validateDefinition definition with
    | Error e, _ -> printfn "Error: %s" e; dict
    | _, Error e -> printfn "Error: %s" e; dict
    | Ok t, Ok d ->
        let key = t.ToLower()
        if dict.ContainsKey key then
            printfn "Word '%s' already exists. Use update instead." t
            dict
        else
            let newDict = CRUD.addWord t d dict
            newDict

let safeUpdateWord (term: string) (definition: string) (dict: Map<string, Word>) : Map<string, Word> =
    match validateTerm term, validateDefinition definition with
    | Error e, _ -> printfn "Error: %s" e; dict
    | _, Error e -> printfn "Error: %s" e; dict
    | Ok t, Ok d ->
        let key = t.ToLower()
        if dict.ContainsKey key then
            let newDict = CRUD.updateWord t d dict
            newDict
        else
            printfn "Word '%s' not found for update." t
            dict

let safeDeleteWord (term: string) (dict: Map<string, Word>) : Map<string, Word> =
    match validateTerm term with
    | Error e -> printfn "Error: %s" e; dict
    | Ok t ->
        let key = t.ToLower()
        if dict.ContainsKey key then
            let newDict = CRUD.deleteWord t dict
            newDict
        else
            printfn "Word '%s' not found for deletion." t
            dict


let safeSearchTerm (term: string) : string option =
    let t = term.Trim()
    if String.IsNullOrWhiteSpace(t) then
        None
    else Some(t.ToLower())


let handleInvalidChoice () : unit =
    printfn "Invalid choice, please try again."


let rec promptValidTerm () : string =
    printf "Enter term: "
    let input = Console.ReadLine()
    match validateTerm input with
    | Ok t -> t
    | Error e -> printfn "Error: %s" e; promptValidTerm ()

let rec promptValidDefinition () : string =
    printf "Enter definition: "
    let input = Console.ReadLine()
    match validateDefinition input with
    | Ok d -> d
    | Error e -> printfn "Error: %s" e; promptValidDefinition ()

