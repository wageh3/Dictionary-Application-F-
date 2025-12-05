open Dictionary_Application.Models

[<EntryPoint>]
let main _ =
    printfn "Dictionary Application \n"
    printfn "All operations auto-save to dict.json and dict.xml\n"

    let mutable dict = Map.empty<string, Word>

    printfn "--- Adding Words ---"
    dict <- CRUD.addWord "Apple" "A kind of fruit" dict
    dict <- CRUD.addWord "Banana" "A yellow fruit" dict
    dict <- CRUD.addWord "Orange" "A citrus fruit" dict
    dict <- CRUD.addWord "Wivi" "A green fruit" dict 
    dict <- CRUD.addWord "strubarry" "A pink fruit" dict
    dict <- CRUD.addWord "Watermelon" "A big fruit" dict
    dict <- CRUD.addWord "lemon" "A yellow green fruit" dict
    dict <- CRUD.addWord "pinaple" "a good fruit" dict
    dict <- CRUD.addWord "pen" "a blue " dict
    printfn "\n--- Current Dictionary ---"
    dict |> Map.iter (fun _ word -> printfn "%s: %s" word.Term word.Definition)

    printfn "\n--- Testing Operations ---"
    
    dict <- CRUD.updateWord "apple" "A sweet red or green fruit" dict
    dict <- CRUD.updateWord "lemon" "A beautifull" dict
    
    dict <- CRUD.deleteWord "Banana" dict
    dict <- CRUD.deleteWord "lemon" dict
    printfn "\n--- Final Result ---"
    printfn "Total words: %d" (Map.count dict)
    
    dict |> Map.iter (fun _ word -> printfn "✨ %s: %s" word.Term word.Definition)

    printfn ("\nCheck dict.json and dict.xml files!")
    0