namespace Dictionary_Application.Models

type UserRole =
    | Admin = 0
    | NormalUser = 1

type User = {
    Username: string
    Password: string
    Role: UserRole
}