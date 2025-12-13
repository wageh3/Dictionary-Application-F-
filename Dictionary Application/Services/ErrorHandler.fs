
module ErrorHandler

open System
open System.Windows.Forms
open System.Text.RegularExpressions
open Dictionary_Application.Models
open CRUD
open FileIO

// --------------------
// Helpers for Validation
// --------------------

let hasVowel (word: string) =
    Regex.IsMatch(word, "[aeiou]")

let isRepeatedChar (word: string) =
    word |> Seq.distinct |> Seq.length = 1

let isMixedLanguage (text: string) =
    let hasEnglish = Regex.IsMatch(text, "[a-zA-Z]")
    let hasArabic  = Regex.IsMatch(text, "[\u0600-\u06FF]")
    hasEnglish && hasArabic

// --------------------
// Normalization Helpers
// --------------------

let irregularMap =
    Map.ofList [
        "went", "go"
        "gone", "go"
        "did", "do"
        "done", "do"
        "saw", "see"
        "seen", "see"
        "ran", "run"
        "ate", "eat"
        "eaten", "eat"
        "was", "be"
        "were", "be"
    ]

let removeSuffix (word: string) =
    if word.EndsWith("ing") && word.Length > 4 then
        word.Substring(0, word.Length - 3)
    elif word.EndsWith("ed") && word.Length > 3 then
        word.Substring(0, word.Length - 2)
    elif word.EndsWith("es") && word.Length > 3 then
        word.Substring(0, word.Length - 2)
    elif word.EndsWith("s") && word.Length > 2 then
        word.Substring(0, word.Length - 1)
    else
        word

// --------------------
// Term Validation
// --------------------

let validateTerm (term: string) : Result<string, string> =
    let t = term.Trim().ToLower()
    if String.IsNullOrWhiteSpace(t) then
        Error "Term cannot be empty."
    elif t.Length < 3 then
        Error "Term is too short to be valid."
    elif not (Regex.IsMatch(t, @"^[a-zA-Z]+$")) then
        Error "Term must contain English letters only."
    elif isRepeatedChar t then
        Error "Term cannot be repeated letters only."
    elif not (hasVowel t) then
        Error "Term does not look like a real word."
    else
        let normalized =
            match irregularMap.TryFind(t) with
            | Some baseForm -> baseForm
            | None -> removeSuffix t
        Ok normalized

// --------------------
// Definition Validation
// --------------------

let validateDefinition (definition: string) : Result<string, string> =
    let d = definition.Trim()
    if String.IsNullOrWhiteSpace(d) then
        Error "Definition cannot be empty."
    elif d.Length < 5 then
        Error "Definition is too short."
    elif Regex.IsMatch(d, @"\d") then
        Error "Definition cannot contain numbers."
    elif Regex.IsMatch(d, @"^(.)\1+$") then
        Error "Definition cannot be meaningless repeated characters."
    elif isMixedLanguage d then
        Error "Definition should not mix Arabic and English."
    else
        Ok d

// --------------------
// Safe Operations (unchanged)
// --------------------

let safeAddWord (term: string) (definition: string) (dict: Map<string, Word>) : Map<string, Word> =
    match validateTerm term, validateDefinition definition with
    | Error e, _ ->
        MessageBox.Show(e, "Invalid Term") |> ignore
        dict
    | _, Error e ->
        MessageBox.Show(e, "Invalid Definition") |> ignore
        dict
    | Ok t, Ok d ->
        let key = t.ToLower()
        if dict.ContainsKey key then
            MessageBox.Show($"Word '{t}' already exists.", "Add Failed") |> ignore
            dict
        else
            CRUD.addWord t d dict

let safeUpdateWord (term: string) (definition: string) (dict: Map<string, Word>) : Map<string, Word> =
    match validateTerm term, validateDefinition definition with
    | Error e, _ ->
        MessageBox.Show(e, "Invalid Term") |> ignore
        dict
    | _, Error e ->
        MessageBox.Show(e, "Invalid Definition") |> ignore
        dict
    | Ok t, Ok d ->
        let key = t.ToLower()
        if dict.ContainsKey key then
            CRUD.updateWord t d dict
        else
            MessageBox.Show($"Word '{t}' not found for update.", "Update Failed") |> ignore
            dict

let safeDeleteWord (term: string) (dict: Map<string, Word>) : Map<string, Word> =
    match validateTerm term with
    | Error e ->
        MessageBox.Show(e, "Invalid Term") |> ignore
        dict
    | Ok t ->
        let key = t.ToLower()
        if dict.ContainsKey key then
            CRUD.deleteWord t dict
        else
            MessageBox.Show($"Word '{t}' not found for deletion.", "Delete Failed") |> ignore
            dict
