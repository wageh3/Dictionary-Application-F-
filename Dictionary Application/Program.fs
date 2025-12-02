open Dictionary_Application.Models
open Dictionary_Application.Services

[<EntryPoint>]
let main _ =
    printfn "Testing CRUD functions...\n"

    
    let dict0 = Map.empty<string, Word>

    
    let dict1 =
        CRUD.addWord "Apple" "A kind of fruit" dict0

    printfn "After Add Word: %A\n" dict1

    
    match CRUD.getWord "Apple" dict1 with
    | Some w -> printfn "Found: %s = %s\n" w.Term w.Definition
    | None -> printfn "Not found!\n"

   
    let dict2 =
        CRUD.updateWord "Apple" "A sweet red or green fruit" dict1

    printfn "After Update: %A\n" dict2

  
    let dict3 =
        CRUD.deleteWord "Apple" dict2

    printfn "After Delete: %A\n" dict3

    0
    