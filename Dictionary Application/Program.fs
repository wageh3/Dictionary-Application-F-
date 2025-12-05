open System
open Dictionary_Application.UI


[<EntryPoint>]
let main argv =
 
    let jsonPath = CRUD.getDataFilePath "dict.json"
    
    let initialDict = 
        match FileIO.loadFromJson jsonPath with
        | Ok loadedDict -> 
            printfn "Dictionary loaded successfully (%d words)." (Map.count loadedDict)
            loadedDict
        | Error msg -> 
            printfn "Notice: %s" msg
            printfn "Starting with a new empty dictionary."
            Map.empty


    Menu.loop initialDict
    
    0 