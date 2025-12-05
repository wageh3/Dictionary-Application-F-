module CRUD

open Dictionary_Application.Models
open System

let basePath = 
    @"C:\Users\DELL\source\repos\wageh3\Dictionary-Application-F-\Dictionary Application\Data"

let addWord (term: string) (definition: string) (dict: Map<string, Word>) =
    let key = term.ToLower()
    let newWord = Word(term, definition)
    let newDict = dict |> Map.add key newWord

    printfn "Added: '%s' - %s" term definition
    printfn "   Total words now: %d" (Map.count newDict)

    FileIO.saveToJson (basePath + @"\dict.json") newDict |> ignore
    FileIO.saveToXml (basePath + @"\dict.xml") newDict |> ignore
    
    newDict

let updateWord (term: string) (newDefinition: string) (dict: Map<string, Word>) =
    let key = term.ToLower()
    match dict.TryFind key with
    | Some existing ->
        let updatedWord = Word(term, newDefinition)
        let newDict = dict |> Map.add key updatedWord
        
        printfn "Updated: '%s' - %s" term newDefinition
        
        FileIO.saveToJson (basePath + @"\dict.json") newDict |> ignore
        FileIO.saveToXml (basePath + @"\dict.xml") newDict |> ignore
        
        newDict
    | None -> 
        printfn "Word '%s' not found for update" term
        dict

let deleteWord (term: string) (dict: Map<string, Word>) =
    let key = term.ToLower()
    let newDict = dict |> Map.remove key
    
    printfn "Deleted: '%s'" term
    printfn "   Total words now: %d" (Map.count newDict)
    
    FileIO.saveToJson (basePath + @"\dict.json") newDict |> ignore
    FileIO.saveToXml (basePath + @"\dict.xml") newDict |> ignore
    
    newDict

let getWord (term: string) (dict: Map<string, Word>) =
    let key = term.ToLower()
    dict |> Map.tryFind key
