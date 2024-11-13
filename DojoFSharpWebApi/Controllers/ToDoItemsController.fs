namespace DojoFSharpWebApi

open Microsoft.AspNetCore.Mvc
open Microsoft.AspNetCore.Authorization
open System.Collections.Generic

[<ApiController>]
[<Route("[controller]")>]
type ToDoItemsController private () = 
    inherit ControllerBase()
    new (toDoListRepository : ToDoListRepository) as this =
        ToDoItemsController () then
        this.toDoListRepository <- toDoListRepository

    [<DefaultValue>]
    val mutable toDoListRepository : ToDoListRepository

    [<HttpGet>]
    member this.Get() =
        ActionResult<IEnumerable<ToDoItem>>(this.toDoListRepository.GetAll())

    [<HttpGet("{id}")>]
    member this.Get(id: int) = 
        if base.ModelState.IsValid then  //check the entry
            if not(this.toDoListRepository.Exists(id)) then //check the existence of the ToDoItem
                base.NotFound("NOT FOUND!, There is no ToDoItem with this code: " + id.ToString()) :> IActionResult
            else
                base.Ok(this.toDoListRepository.GetById(id)) :> IActionResult
        else
            base.BadRequest(base.ModelState) :> IActionResult

    [<HttpPost>]
    member this.Post([<FromBody>] newItem: ToDoItem) =
        if (base.ModelState.IsValid) then
            if not( isNull newItem.Name ) then
                if(newItem.Id <> 0) then //check if the ID is set
                    base.BadRequest("BAD REQUEST, the ToDoItemID is autoincremented") :> IActionResult
                else 
                    this.toDoListRepository.Add(newItem) |> ignore
                    base.Ok(this.toDoListRepository.Last()) :> IActionResult
            else
                base.BadRequest("BAD REQUEST!, the field Initials can not be null") :> IActionResult         
        else
            base.BadRequest(base.ModelState) :> IActionResult

    [<HttpPut("{id}")>]
     member this.Put(id: int, [<FromBody>] _ToDoItem: ToDoItem) =
        if (base.ModelState.IsValid) then 
            if not(isNull _ToDoItem.Name) then
                if(_ToDoItem.Id <> id) then 
                    base.BadRequest() :> IActionResult
                else
                    try
                        this.toDoListRepository.Update(_ToDoItem) |> ignore
                        base.Ok(_ToDoItem) :> IActionResult
                    with ex ->
                        if not(this.toDoListRepository.Exists(id)) then
                            base.NotFound() :> IActionResult
                        else 
                            base.BadRequest() :> IActionResult
            else
                base.BadRequest() :> IActionResult                     
        else    
            base.BadRequest(base.ModelState) :> IActionResult
            
    [<Authorize>]
    [<HttpDelete("{id}")>]
    member this.Delete(id: int) =
        if (base.ModelState.IsValid) then
            if not(this.toDoListRepository.Exists(id)) then
                base.NotFound() :> IActionResult
            else
                this.toDoListRepository.Remove(id) |> ignore
                base.Ok(this.toDoListRepository.Last()) :> IActionResult
        else
            base.BadRequest(base.ModelState) :> IActionResult
    
    [<Authorize("RequireAdminFromZDI")>]
    [<HttpPost("reset")>]
    member this.Post() =
        this.toDoListRepository.Reset()
        base.Ok("Database has been reset!") :> IActionResult