namespace Dictionary_Application.Models

type Word() =
    member val Term: string = "" with get, set
    member val Definition: string = "" with get, set

    new(term: string, definition: string) as this =
        Word() then
        this.Term <- term
        this.Definition <- definition

    override this.ToString() =
        sprintf "{ Term = \"%s\"; Definition = \"%s\" }" this.Term this.Definition