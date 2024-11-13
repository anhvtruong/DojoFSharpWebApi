namespace DojoFSharpWebApi

open Microsoft.EntityFrameworkCore

type DatabaseContext(options : DbContextOptions<DatabaseContext>) = 
    inherit DbContext(options)
    
    [<DefaultValue>] val mutable toDoItems : DbSet<ToDoItem>
    member public this.ToDoItems with get() = this.toDoItems and set value = this.toDoItems <- value
        
    override _.OnModelCreating builder =
        builder.Entity<ToDoItem>().HasData(
            [|
                { Id = 1; Name = "Check the 12-hour budget"; IsComplete = true }
                { Id = 2; Name = "Download the dojo invitation"; IsComplete = true }
                { Id = 3; Name = "Join the dojo"; IsComplete = false }
            |]) |> ignore

    override __.OnConfiguring(options: DbContextOptionsBuilder) : unit =
        options.UseSqlite("Data Source=ToDoList.db") |> ignore