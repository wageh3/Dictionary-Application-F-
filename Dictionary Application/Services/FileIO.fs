module FileIO

open Dictionary_Application.Models
open System
open System.IO
open System.Text.Json
open System.Xml.Serialization


let saveGeneric<'T> (filePath: string) (data: 'T) (options: JsonSerializerOptions) =
    try
        let json = JsonSerializer.Serialize(data, options)
        File.WriteAllText(filePath, json)
        Ok "Data saved successfully."
    with
    | ex -> Error $"Failed to save JSON: {ex.Message}"


let saveToXml (filePath: string) (dict: Map<string, Word>) =
    try
        let sortedDict = dict |> Map.toSeq |> Seq.sortBy fst |> Map.ofSeq
        let data = sortedDict |> Map.toList |> List.map snd |> Array.ofList
        let serializer = XmlSerializer(typeof<Word array>)
        use writer = new StreamWriter(filePath)
        serializer.Serialize(writer, data)
        Ok "XML saved successfully."
    with
    | ex -> Error $"Failed to save XML: {ex.Message}"


let loadFromJson (filePath: string) : Result<Map<string, Word>, string> =
    try
        if File.Exists(filePath) then
            let json = File.ReadAllText(filePath)
            let words = JsonSerializer.Deserialize<Word array>(json)
            let dict =
                match words with
                | null -> Map.empty
                | arr -> arr |> Array.map (fun w -> (w.Term.ToLower(), w)) |> Map.ofArray
            Ok (dict |> Map.toSeq |> Seq.sortBy fst |> Map.ofSeq)
        else
            Ok Map.empty<string, Word>
    with
    | ex -> Error $"Failed to load JSON: {ex.Message}"


let loadFromXml (filePath: string) : Result<Map<string, Word>, string> =
    try
        if File.Exists(filePath) then
            let serializer = XmlSerializer(typeof<Word array>)
            use reader = new StreamReader(filePath)
            let words = serializer.Deserialize(reader) :?> Word array
            let dict = words |> Array.map (fun w -> (w.Term.ToLower(), w)) |> Map.ofArray
            Ok (dict |> Map.toSeq |> Seq.sortBy fst |> Map.ofSeq)
        else
            Ok Map.empty<string, Word>
    with
    | ex -> Error $"Failed to load XML: {ex.Message}"