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
    

let addWord (term: string) (definition: string) (dict: Map<string, Word>) =
    if Map.containsKey (term.ToLower()) dict then
        printfn "Word '%s' already exists. Use update instead." term
        dict
    elif String.IsNullOrWhiteSpace(term) then
        printfn "Term cannot be empty."
        dict
    elif String.IsNullOrWhiteSpace(definition) then
        printfn "Definition cannot be empty."
        dict
    else
        let key = term.ToLower()
        let newWord = Word(term, definition)
        let newDict = dict |> Map.add key newWord


        FileIO.saveToJson (getDataFilePath "dict.json") newDict |> ignore
        FileIO.saveToXml (getDataFilePath "dict.xml") newDict |> ignore
    
        newDict

let updateWord (term: string) (newDefinition: string) (dict: Map<string, Word>) =
    let key = term.ToLower()
    match dict.TryFind key with
    | Some existing ->
        let updatedWord = Word(term, newDefinition)
        let newDict = dict |> Map.add key updatedWord
        
        printfn "Updated: '%s' - %s" term newDefinition
        
        FileIO.saveToJson (getDataFilePath "dict.json") newDict |> ignore
        FileIO.saveToXml (getDataFilePath "dict.xml") newDict |> ignore
        
        newDict
    | None -> 
        printfn "Word '%s' not found for update" term
        dict

let deleteWord (term: string) (dict: Map<string, Word>) =
    let key = term.ToLower()
    let newDict = dict |> Map.remove key
    
    printfn "Deleted: '%s'" term
    printfn "   Total words now: %d" (Map.count newDict)
    
    FileIO.saveToJson (getDataFilePath "dict.json") newDict |> ignore
    FileIO.saveToXml (getDataFilePath "dict.xml") newDict |> ignore
    
    newDict
/// Get a word
let getWord (term: string) (dict: Map<string, Word>) =
    let key = term.ToLower()
    dict |> Map.tryFind key
