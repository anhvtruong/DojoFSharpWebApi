namespace DojoFSharpWebApi

open System.Linq;

type ToDoListRepository (context: DatabaseContext) =

    member this.Reset() = 
        context.Database.EnsureDeleted() |> ignore
        context.Database.EnsureCreated() |> ignore

    member this.GetAll() = context.ToDoItems.ToList()

    member this.GetById(id: int) = context.ToDoItems.FirstOrDefault(fun x -> x.Id = id)

    member this.Exists(id: int) = context.ToDoItems.Any(fun x -> x.Id = id)

    member this.Add(newItem: ToDoItem) =
        context.ToDoItems.Add(newItem) |> ignore
        context.SaveChanges()

    member this.Update(newItem: ToDoItem) =
        context.ToDoItems.Update(newItem) |> ignore
        context.SaveChanges()

    member this.Remove(id: int) = 
        context.ToDoItems.Remove(this.GetById(id)) |> ignore
        context.SaveChanges()

    member this.Last() = context.ToDoItems.OrderBy(fun x -> x.Id).Last()