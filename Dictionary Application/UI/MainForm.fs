module MainForm

open System
open System.Drawing
open System.Windows.Forms
open Dictionary_Application.Models
open CRUD
open ErrorHandler
open FileIO
open Search

type MainForm() as this =
    inherit Form(Text = "F# Dictionary App", Width = 750, Height = 600)

    // FORM SETTINGS
    do
        this.BackColor <- Color.FromArgb(235, 242, 255)
        this.Font <- new Font("Segoe UI", 10.0f)
        this.FormBorderStyle <- FormBorderStyle.FixedSingle
        this.MaximizeBox <- false

    // INPUTS 
    let lblTerm = new Label(Text = "Term:", Top = 20, Left = 20, Width = 60)
    let txtTerm = new TextBox(Top = 20, Left = 90, Width = 250)

    let lblDef = new Label(Text = "Definition:", Top = 60, Left = 20, Width = 80)
    let txtDef = new TextBox(Top = 60, Left = 110, Width = 250, Height = 60, Multiline = true)

    //  ACTION BUTTONS 
    let buttonFont = new Font("Segoe UI", 10.0f, FontStyle.Bold)

    let btnAdd = new Button(Text = "Add", Top = 130, Left = 20, Width = 100,
                            BackColor = Color.MediumSeaGreen, ForeColor = Color.White, Font = buttonFont)

    let btnUpdate = new Button(Text = "Update", Top = 130, Left = 140, Width = 100,
                               BackColor = Color.Goldenrod, ForeColor = Color.White, Font = buttonFont)

    let btnDelete = new Button(Text = "Delete", Top = 130, Left = 260, Width = 100,
                               BackColor = Color.IndianRed, ForeColor = Color.White, Font = buttonFont)

    let btnPrintAll = new Button(Text = "Print All", Top = 130, Left = 380, Width = 110,
                                 BackColor = Color.SteelBlue, ForeColor = Color.White, Font = buttonFont)

    // SEARCH BAR 
    let lblSearch = new Label(Text = "Search:", Top = 210, Left = 20, Width = 60)
    let txtSearch = new TextBox(Top = 210, Left = 90, Width = 250)

    let btnSearchPartial =
        new Button(Text = "Partial", Top = 208, Left = 350, Width = 90,
                   BackColor = Color.MediumPurple, ForeColor = Color.White, Font = buttonFont)

    let btnSearchExact =
        new Button(Text = "Exact", Top = 208, Left = 450, Width = 90,
                   BackColor = Color.CornflowerBlue, ForeColor = Color.White, Font = buttonFont)

    //  LIST BOX 
    let lstWords =
        new ListBox(Top = 260, Left = 20, Width = 690, Height = 310,
                    BackColor = Color.White, ForeColor = Color.Black,
                    Font = new Font("Consolas", 11.0f))

    // DICTIONARY STATE 
    let mutable dict : Map<string, Word> =
        match FileIO.loadFromJson (CRUD.getDataFilePath "dict.json") with
        | Ok d -> d
        | Error _ -> Map.empty

    //  INTERNAL FUNCTIONS 
    let refreshList() =
        lstWords.Items.Clear()
        lstWords.Items.Add(sprintf "%-20s %-50s" "Term" "Definition") |> ignore
        for (k, v) in dict |> Map.toList |> List.sortBy fst do
            lstWords.Items.Add(sprintf "%-20s %-50s" k v.Definition) |> ignore

    let clearInputs() =
        txtTerm.Text <- ""
        txtDef.Text <- ""
        txtSearch.Text <- ""

    let saveDict() =
        FileIO.saveToJson (CRUD.getDataFilePath "dict.json") dict |> ignore
        FileIO.saveToXml (CRUD.getDataFilePath "dict.xml") dict |> ignore

    // ADD 
    do
        this.Controls.AddRange(
            [| lblTerm; txtTerm; lblDef; txtDef;
               btnAdd; btnUpdate; btnDelete; btnPrintAll;
               lblSearch; txtSearch; btnSearchPartial; btnSearchExact;
               lstWords |])

    // ========= EVENTS =========

    // Add Word
    do btnAdd.Click.Add(fun _ ->
        let term = txtTerm.Text.Trim()
        let def = txtDef.Text.Trim()

        match validateTerm term, validateDefinition def with
        | Error e, _ -> MessageBox.Show(e) |> ignore
        | _, Error e -> MessageBox.Show(e) |> ignore
        | Ok t, Ok d ->
            if dict.ContainsKey(t.ToLower()) then
                MessageBox.Show("Already exists!") |> ignore
            else
                dict <- CRUD.addWord t d dict
                saveDict()
                clearInputs()
    )

    // Update
    do btnUpdate.Click.Add(fun _ ->
        let term = txtTerm.Text.Trim()
        let def = txtDef.Text.Trim()

        match validateTerm term, validateDefinition def with
        | Error e, _ -> MessageBox.Show(e) |> ignore
        | _, Error e -> MessageBox.Show(e) |> ignore
        | Ok t, Ok d ->
            if dict.ContainsKey(t.ToLower()) then
                dict <- CRUD.updateWord t d dict
                saveDict()
                clearInputs()
            else MessageBox.Show("Word not found.") |> ignore
    )

    // Delete
    do btnDelete.Click.Add(fun _ ->
        let term = txtTerm.Text.Trim()

        match validateTerm term with
        | Error e -> MessageBox.Show(e) |> ignore
        | Ok t ->
            if dict.ContainsKey(t.ToLower()) then
                dict <- CRUD.deleteWord t dict
                saveDict()
                clearInputs()
            else MessageBox.Show("Not found.") |> ignore
    )

    // PARTIAL Search
    do btnSearchPartial.Click.Add(fun _ ->
        let text = txtSearch.Text.Trim()
        let results = partialSearch text dict

        if results.IsEmpty then
            MessageBox.Show("No partial matches.") |> ignore
        else
            let msg = results |> List.map (fun w -> $"{w.Term}: {w.Definition}") |> String.concat "\n"
            MessageBox.Show(msg) |> ignore
    )

    // EXACT Search
    do btnSearchExact.Click.Add(fun _ ->
        let text = txtSearch.Text.Trim()
        match caseInsensitiveSearch text dict with
        | Some w -> MessageBox.Show($"{w.Term}: {w.Definition}") |> ignore
        | _ -> MessageBox.Show("No exact match.") |> ignore
    )

    // Print All
    do btnPrintAll.Click.Add(fun _ -> refreshList())

// ENTRY POINT
[<EntryPoint>]
let main argv =
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault(false)
    Application.Run(new MainForm())
    0
