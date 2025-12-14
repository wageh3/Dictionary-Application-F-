module CRUD

open Dictionary_Application.Models
open System
open System.IO


let getDataFilePath (fileName: string) =
    let currentDir = AppDomain.CurrentDomain.BaseDirectory
    let dataFolder = Path.Combine(currentDir, "Data")
    if not (Directory.Exists(dataFolder)) then
        Directory.CreateDirectory(dataFolder) |> ignore
    Path.Combine(dataFolder, fileName)


let private sortDict (dict: Map<string, Word>) =
    dict |> Map.toSeq 
         |> Seq.sortBy fst 
         |> Map.ofSeq


let addWord (term: string) (definition: string) (dict: Map<string, Word>) : Map<string, Word> =
    let key = term.ToLower()
    if dict.ContainsKey key then dict
    else
        dict |> Map.add key (Word(term, definition))
             |> sortDict


let updateWord (term: string) (newDefinition: string) (dict: Map<string, Word>) : Map<string, Word> =
    let key = term.ToLower()
    match dict.TryFind key with
    | Some _ ->
        dict |> Map.add key (Word(term, newDefinition)) 
             |> sortDict
    
    | None -> dict


let deleteWord (term: string) (dict: Map<string, Word>) =
    let key = term.ToLower()
    dict |> Map.remove key 
         |> sortDict
   


let getWord (term: string) (dict: Map<string, Word>) : Word option =
    let key = term.ToLower()
    dict |> Map.tryFind key
