namespace Dictionary_Application.UI
open Dictionary_Application 
open Dictionary_Application.Models

module Menu =
    let rec loop (dict: Map<string, Word>) =
        printfn "\n--- Dictionary Menu ---"
        printfn "1. Add Word"
        printfn "2. Update Word"
        printfn "3. Delete Word"
        printfn "4. Search (Exact)"      
        printfn "5. Search (Partial)"
        printfn "6. Load (Reload from file)"
        printfn "7. Print All the Dict"
        printfn "0. Exit"

        printf "Choice: "
        let choice = System.Console.ReadLine()

        match choice with
        | "1" ->
            printf "Enter term: "
            let term = System.Console.ReadLine()

            printf "Enter definition: "
            let def = System.Console.ReadLine()

            let newDict = CRUD.addWord term def dict
            loop newDict

        | "2" ->
            printf "Term to update: "
            let term = System.Console.ReadLine()

            printf "New definition: "
            let def = System.Console.ReadLine()

            let newDict = CRUD.updateWord term def dict
            loop newDict

        | "3" ->
            printf "Term to delete: "
            let term = System.Console.ReadLine()

            let newDict = CRUD.deleteWord term dict
            loop newDict

        | "4" ->
            printf "Enter term to search: "
            let term = System.Console.ReadLine()
            
            
            match Search.caseInsensitiveSearch term dict with
            | Some word -> 
                printfn "\nFound Details:\n"
                printfn $"Term: {word.Term}"  
                printfn $"Definition: {word.Definition}\n" 
            | None -> 
                printfn "Word not found."
            
            loop dict
        | "5" ->
            printf "Enter part of the word: "
            let pattern = System.Console.ReadLine()
            
            
            let results = Search.partialSearch pattern dict
            
            if results.IsEmpty then
                printfn "No matches found."
            else
                printfn $"\nFound {results.Length} matches:\n" 
                
                results |> List.iter (fun w -> printfn "- %s: %s" w.Term w.Definition)
            
            loop dict
        | "6" ->
            
            let jsonPath = CRUD.getDataFilePath "dict.json"
            
            match FileIO.loadFromJson jsonPath with
            | Ok loadedDict ->
                printfn "Dictionary loaded successfully (%d words)." (Map.count loadedDict)
                loop loadedDict
            | Error msg ->
                printfn "Error loading dictionary: %s" msg
                
                loop dict
        | "7" ->
            if Map.isEmpty dict then
                printfn "The dictionary is currently empty."
            else
                printfn "\nCurrent Dictionary Contents:\n"
                dict
                |> Map.toList
                |> List.iter (fun (_, w) -> printfn $"- {w.Term}: {w.Definition} \n"  )
            loop dict
        | "0" -> 
            printfn "Goodbye!"
           

        | _ -> 
            printfn "Invalid choice, please try again."
            loop dict