namespace Dictionary_Application.UI
open System
open Dictionary_Application.Models
open CRUD
open ErrorHandler
open Search
open FileIO

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
        let choice = Console.ReadLine()

        match choice with
        | "1" ->
            let term = promptValidTerm ()
            if dict.ContainsKey(term.ToLower()) then
                printfn "Word '%s' already exists. Use update instead." term
                loop dict
            else
                let def = promptValidDefinition ()
                let newDict = safeAddWord term def dict
                printfn "Added: '%s' - %s" term def
                printfn "Total words now: %d" (Map.count newDict)
                loop newDict

        | "2" ->
            let term = promptValidTerm ()
            if not (dict.ContainsKey(term.ToLower())) then
                printfn "Word '%s' not found. Use add instead." term
                loop dict
            else
                let def = promptValidDefinition ()
                let newDict = safeUpdateWord term def dict
                printfn "Updated: '%s' - %s" term def
                printfn "Total words now: %d" (Map.count newDict)
                loop newDict

        | "3" ->
            let term = promptValidTerm ()
            let newDict = safeDeleteWord term dict
            printfn "Total words now: %d" (Map.count newDict)
            loop newDict

        | "4" ->
            printf "Enter term to search: "
            let term = Console.ReadLine()
            match safeSearchTerm term with
            | Some t ->
                match caseInsensitiveSearch t dict with
                | Some w ->
                    printfn "\nFound Details:\nTerm: %s\nDefinition: %s\n" w.Term w.Definition
                | None -> printfn "Word not found."
            | None -> printfn "Search term cannot be empty."
            loop dict

        | "5" ->
            printf "Enter part of the word: "
            let pattern = Console.ReadLine()
            let results = partialSearch pattern dict
            if results.IsEmpty then
                printfn "No matches found."
            else
                printfn "\nFound %d matches:\n" results.Length
                results |> List.iter (fun w -> printfn "- %s: %s" w.Term w.Definition)
            loop dict

        | "6" ->
            let jsonPath = CRUD.getDataFilePath "dict.json"
            match loadFromJson jsonPath with
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
                |> List.iter (fun (_, w) -> printfn "- %s: %s" w.Term w.Definition)
                printfn "\nTotal words: %d" (Map.count dict)
            loop dict

        | "0" ->
            printfn "Goodbye!"

        | _ ->
            handleInvalidChoice()
            loop dict
