module Search


open Dictionary_Application.Models 
open System

let caseInsensitiveSearch (term: string) (dict: Map<string, Word>) =
    let searchKey = term.Trim().ToLower()
    dict |> Map.tryFind searchKey

let partialSearch (pattern: string) (dict: Map<string, Word>) =
    if String.IsNullOrWhiteSpace(pattern) then
        []
    else
        let searchPattern = pattern.Trim().ToLower()
        
        dict
        |> Map.toList
        |> List.filter (fun (k, _) -> k.Contains(searchPattern))
        |> List.map snd