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

    printfn "\n--- Testing Operations ---"
    
    dict <- CRUD.updateWord "apple" "A sweet red or green fruit" dict
    dict <- CRUD.updateWord "lemon" "A beautifull" dict
    dict <- CRUD.updateWord "pen" "A blue colorrr" dict
    dict <- CRUD.deleteWord "Banana" dict
    dict <- CRUD.deleteWord "lemon" dict
    printfn "\n--- Final Result ---"
    printfn "Total words: %d" (Map.count dict)
    
    dict |> Map.iter (fun _ word -> printfn " %s: %s" word.Term word.Definition)

    0
    