
module MainForm

open System
open System.Drawing
open System.Windows.Forms
open Dictionary_Application.Models
open Dictionary_Application.Services
open CRUD
open ErrorHandler
open FileIO
open Search

// ==========================================
// 1. REGISTER FORM
// ==========================================
type RegisterForm() as this =
    inherit Form(Text = "Register New User", Width = 400, Height = 350)

    do
        this.StartPosition <- FormStartPosition.CenterScreen
        this.FormBorderStyle <- FormBorderStyle.FixedDialog
        this.MaximizeBox <- false

    let lblUser = new Label(Text = "Username:", Top = 30, Left = 30, Width = 80)
    let txtUser = new TextBox(Top = 30, Left = 120, Width = 200)

    let lblPass = new Label(Text = "Password:", Top = 80, Left = 30, Width = 80)
    let txtPass = new TextBox(Top = 80, Left = 120, Width = 200, PasswordChar = '*')

    let lblRole = new Label(Text = "Role:", Top = 130, Left = 30, Width = 80)
    let cmbRole = new ComboBox(Top = 130, Left = 120, Width = 200)
    do cmbRole.Items.AddRange([| "User"; "Admin" |])
    do cmbRole.SelectedIndex <- 0

    let btnReg = new Button(Text = "Register", Top = 200, Left = 120, Width = 90, BackColor = Color.MediumSeaGreen, ForeColor = Color.White)
    let btnCancel = new Button(Text = "Cancel", Top = 200, Left = 230, Width = 90, BackColor = Color.IndianRed, ForeColor = Color.White)

    do
        this.Controls.AddRange([| lblUser; txtUser; lblPass; txtPass; lblRole; cmbRole; btnReg; btnCancel |])

    // Events
    member this.BtnRegister = btnReg
    member this.BtnCancel = btnCancel

    override this.OnLoad(e) =
        base.OnLoad(e)
        this.BtnRegister.Click.Add(fun _ -> 
            if String.IsNullOrWhiteSpace(txtUser.Text) || String.IsNullOrWhiteSpace(txtPass.Text) then
                MessageBox.Show("Please fill all fields.") |> ignore
            else
                let selectedRole = if cmbRole.Text = "Admin" then UserRole.Admin else UserRole.NormalUser
                match UserAuth.register txtUser.Text txtPass.Text selectedRole with
                | Ok _ -> 
                    MessageBox.Show("Registration Successful! Please Login.") |> ignore
                    this.Close()
                | Error msg -> MessageBox.Show(msg) |> ignore
        )
        btnCancel.Click.Add(fun _ -> this.Close())

// ==========================================
// 2. MAIN FORM
// ==========================================
type MainForm(currentUser: User) as this = 
    inherit Form(Text = sprintf "F# Dictionary App - Welcome %s (%O)" currentUser.Username currentUser.Role, Width = 750, Height = 600)

    do
        this.StartPosition <- FormStartPosition.CenterScreen
        this.BackColor <- Color.FromArgb(235, 242, 255)
        this.Font <- new Font("Segoe UI", 10.0f)
        this.FormBorderStyle <- FormBorderStyle.FixedSingle
        this.MaximizeBox <- false

    // INPUT CONTROLS
    let lblTerm = new Label(Text = "Term:", Top = 20, Left = 20, Width = 60)
    let txtTerm = new TextBox(Top = 20, Left = 90, Width = 250)

    let lblDef = new Label(Text = "Definition:", Top = 60, Left = 20, Width = 80)
    let txtDef = new TextBox(Top = 60, Left = 110, Width = 250, Height = 60, Multiline = true)

    // ACTION BUTTONS
    let buttonFont = new Font("Segoe UI", 10.0f, FontStyle.Bold)

    let btnAdd = new Button(Text = "Add", Top = 130, Left = 20, Width = 100,
                            BackColor = Color.MediumSeaGreen, ForeColor = Color.White, Font = buttonFont)

    let btnUpdate = new Button(Text = "Update", Top = 130, Left = 140, Width = 100,
                               BackColor = Color.Goldenrod, ForeColor = Color.White, Font = buttonFont)

    let btnDelete = new Button(Text = "Delete", Top = 130, Left = 260, Width = 100,
                               BackColor = Color.IndianRed, ForeColor = Color.White, Font = buttonFont)

    let btnPrintAll = new Button(Text = "Print All", Top = 130, Left = 380, Width = 110,
                                 BackColor = Color.SteelBlue, ForeColor = Color.White, Font = buttonFont)

    let btnLogout = new Button(Text = "Logout", Top = 130, Left = 550, Width = 100,
                               BackColor = Color.Gray, ForeColor = Color.White, Font = buttonFont)

    // SEARCH
    let lblSearch = new Label(Text = "Search:", Top = 210, Left = 20, Width = 60)
    let txtSearch = new TextBox(Top = 210, Left = 90, Width = 250)

    let btnSearchPartial = new Button(Text = "Partial", Top = 208, Left = 350, Width = 90,
                                      BackColor = Color.MediumPurple, ForeColor = Color.White, Font = buttonFont)

    let btnSearchExact = new Button(Text = "Exact", Top = 208, Left = 450, Width = 90,
                                    BackColor = Color.CornflowerBlue, ForeColor = Color.White, Font = buttonFont)

    // LISTBOX
    let lstWords = new ListBox(Top = 260, Left = 20, Width = 690, Height = 310,
                               BackColor = Color.White, ForeColor = Color.Black,
                               Font = new Font("Consolas", 11.0f))

    // DICTIONARY STATE
    let mutable dict : Map<string, Word> =
        match FileIO.loadFromJson (CRUD.getDataFilePath "dict.json") with
        | Ok d -> d
        | Error _ -> Map.empty

    // REFRESH LIST UI
    let refreshList() =
        lstWords.Items.Clear()
        lstWords.Items.Add(sprintf "%-20s %-50s" "Term" "Definition") |> ignore
        for (k, v) in dict |> Map.toList |> List.sortBy fst do
            lstWords.Items.Add(sprintf "%-20s %-50s" k v.Definition) |> ignore

    let clearInputs() =
        txtTerm.Text <- ""
        txtDef.Text <- ""
        txtSearch.Text <- ""

    let displayResults (results: List<Word>) =
        lstWords.Items.Clear()
        lstWords.Items.Add(sprintf "%-20s %-50s" "Term" "Definition") |> ignore
        if results.IsEmpty then
            lstWords.Items.Add("--- No results found. ---") |> ignore
        else
            for w in results |> List.sortBy (fun w -> w.Term) do
                lstWords.Items.Add(sprintf "%-20s %-50s" w.Term w.Definition) |> ignore

    let saveDict() =
        FileIO.saveGeneric (CRUD.getDataFilePath "dict.json") dict (getJsonOptions()) |> ignore
        FileIO.saveToXml (CRUD.getDataFilePath "dict.xml") dict |> ignore

    // ADD CONTROLS TO FORM
    do
        let commonControls : Control[] = [| lblTerm; txtTerm; lblDef; txtDef;
                                            lblSearch; txtSearch; btnSearchPartial; btnSearchExact; lstWords |]

        this.Controls.AddRange(commonControls)

        if currentUser.Role = UserRole.Admin then
            let adminControls : Control[] = [| btnAdd; btnUpdate; btnDelete; btnPrintAll; btnLogout |]
            this.Controls.AddRange(adminControls)
        else
            let startLeft = 20
            let topPos = 130
            let spacing = 120

            btnAdd.Left <- startLeft
            btnAdd.Top <- topPos

            btnPrintAll.Left <- startLeft + spacing
            btnPrintAll.Top <- topPos

            btnLogout.Left <- startLeft + spacing * 2
            btnLogout.Top <- topPos

            this.Controls.AddRange([| btnAdd; btnPrintAll; btnLogout |])
            this.Text <- this.Text + " [Read Only Mode]"

    // EVENTS =========================================
    do
        btnAdd.Click.Add(fun _ ->
            let newDict = safeAddWord txtTerm.Text txtDef.Text dict
            if not (obj.ReferenceEquals(newDict, dict)) then
                dict <- newDict
                saveDict()
                clearInputs()
                refreshList()
        )

        btnUpdate.Click.Add(fun _ ->
            if MessageBox.Show("Are you sure you want to update this term?", "Confirm Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes then
                let newDict = safeUpdateWord txtTerm.Text txtDef.Text dict
                if not (obj.ReferenceEquals(newDict, dict)) then
                    dict <- newDict
                    saveDict()
                    clearInputs()
                    refreshList()
        )

        btnDelete.Click.Add(fun _ ->
            if MessageBox.Show("Are you sure you want to delete this term?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes then
                let newDict = safeDeleteWord txtTerm.Text dict
                if not (obj.ReferenceEquals(newDict, dict)) then
                    dict <- newDict
                    saveDict()
                    clearInputs()
                    refreshList()
        )

        btnSearchPartial.Click.Add(fun _ ->
            let results = partialSearch txtSearch.Text dict
            displayResults results
            if results.IsEmpty then MessageBox.Show("No partial matches.") |> ignore
        )

        btnSearchExact.Click.Add(fun _ ->
            match caseInsensitiveSearch txtSearch.Text dict with
            | Some w -> displayResults [w]
            | None -> displayResults []; MessageBox.Show("No exact match.") |> ignore
        )

        btnPrintAll.Click.Add(fun _ -> refreshList())
        btnLogout.Click.Add(fun _ -> this.Close())

    override this.OnLoad(e: EventArgs) =
        base.OnLoad(e)
        refreshList()

// ==========================================
// 3. LOGIN FORM
// ==========================================
type LoginForm() as this =
    inherit Form(Text = "Login", Width = 400, Height = 300)

    do
        this.StartPosition <- FormStartPosition.CenterScreen
        this.FormBorderStyle <- FormBorderStyle.FixedSingle
        this.MaximizeBox <- false

    let lblUser = new Label(Text = "Username:", Top = 40, Left = 40, Width = 80)
    let txtUser = new TextBox(Top = 40, Left = 130, Width = 200)

    let lblPass = new Label(Text = "Password:", Top = 90, Left = 40, Width = 80)
    let txtPass = new TextBox(Top = 90, Left = 130, Width = 200, PasswordChar = '*')

    let btnLogin = new Button(Text = "Login", Top = 150, Left = 130, Width = 90, BackColor = Color.SteelBlue, ForeColor = Color.White)
    let btnGoToReg = new Button(Text = "Register", Top = 150, Left = 240, Width = 90, BackColor = Color.Gray, ForeColor = Color.White)

    do this.Controls.AddRange([| lblUser; txtUser; lblPass; txtPass; btnLogin; btnGoToReg |])

    override this.OnLoad(e) =
        base.OnLoad(e)
        btnLogin.Click.Add(fun _ ->
            match UserAuth.login txtUser.Text txtPass.Text with
            | Ok user ->
                this.Hide()
                use mainForm = new MainForm(user)
                mainForm.ShowDialog() |> ignore
                this.Show()
                txtPass.Text <- ""
            | Error msg -> MessageBox.Show(msg, "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error) |> ignore
        )
        btnGoToReg.Click.Add(fun _ ->
            use regForm = new RegisterForm()
            regForm.ShowDialog() |> ignore
        )

// ==========================================
// ENTRY POINT
// ==========================================
[<EntryPoint>]
let main argv =
    Application.EnableVisualStyles()
    Application.SetCompatibleTextRenderingDefault(false)
    Application.Run(new LoginForm())
    0
