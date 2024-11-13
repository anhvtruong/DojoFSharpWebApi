namespace DojoFSharpWebApi

open System.ComponentModel.DataAnnotations

[<CLIMutable>]
type ToDoItem =
    {
        [<Key>]
        Id : int

        [<Required>]
        [<MaxLength(50)>]
        Name : string

        [<Required>]
        IsComplete : bool
    }