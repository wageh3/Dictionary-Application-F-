namespace Dictionary_Application.Services

open System.IO
open System.Text.Json
open System.Text.Json.Serialization 
open Dictionary_Application.Models
open CRUD
open FileIO

module UserAuth =

    let private usersFile = CRUD.getDataFilePath "users.json"

    // دالة مساعدة لعمل إعدادات الـ JSON
    // دي اللي بتحل مشكلة الـ Enum/Union
    let private getJsonOptions () =
        let options = JsonSerializerOptions(WriteIndented = true)
        options.Converters.Add(JsonStringEnumConverter())
        options

    /// تحميل اليوزرز من JSON
    let loadUsers () =
        if File.Exists(usersFile) then
            try
                let json = File.ReadAllText(usersFile) 
                JsonSerializer.Deserialize<User list>(json, getJsonOptions())
            with
            | _ -> 
                []
        else
            []

    /// حفظ اليوزرز في JSON
    let saveUsers (users: User list) =
        FileIO.saveGeneric usersFile users (getJsonOptions()) |> ignore

    /// Register
    let register username password role =
        let users = loadUsers()

        if users |> List.exists (fun u -> u.Username = username) then
            Error "Username already exists"
        else
            let newUser = { Username = username; Password = password; Role = role }
            let updated = newUser :: users
            saveUsers updated
            Ok newUser

    /// Login
    let login username password =
        let users = loadUsers()
        match users |> List.tryFind (fun u -> u.Username = username && u.Password = password) with
        | Some user -> Ok user
        | None -> Error "Invalid username or password"