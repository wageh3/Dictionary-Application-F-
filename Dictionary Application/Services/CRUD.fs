namespace Dictionary_Application.Services

open Dictionary_Application.Models

module CRUD =

    /// Add a new word.
    let addWord (term: string) (definition: string) (dict: Map<string, Word>) =
        let key = term.ToLower()
        let newWord = { Term = term; Definition = definition }
        dict |> Map.add key newWord


    /// Update
    let updateWord (term: string) (newDefinition: string) (dict: Map<string, Word>) =
        let key = term.ToLower()

        match dict.TryFind key with
        | Some existing ->
            let updated = { existing with Definition = newDefinition }
            dict |> Map.add key updated
        | None ->
            dict   


    /// Delete 
    let deleteWord (term: string) (dict: Map<string, Word>) =
        let key = term.ToLower()
        dict |> Map.remove key


    /// Get a word
    let getWord (term: string) (dict: Map<string, Word>) =
        let key = term.ToLower()
        dict |> Map.tryFind key
