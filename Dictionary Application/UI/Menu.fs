namespace Dictionary_Application.UI
open Dictionary_Application 
open Dictionary_Application.Models

module Menu =
    let rec loop (dict: Map<string, Word>) =
        printfn "\n--- Dictionary Menu ---"
