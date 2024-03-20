using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ToDoController : ControllerBase{
    private MongoClient _mongoClient;
    private IMongoDatabase _db;

    public ToDoController(MongoClient mongoClient){
        _mongoClient = mongoClient;
        _db = _mongoClient.GetDatabase("todoDB");
    }
    

    [HttpGet]
    public List<ToDo> Fetch(){  
        string UserName =  User.Identity.Name;
        List<ToDo> todos = _db.GetCollection<ToDo>(UserName).Find(_ => true).ToList();
        return todos;

    }


    [HttpPost]
    public String Save(ToDo toDo){
        //fetching current logged in user

       string UserName =  User.Identity.Name;
       
        _db.GetCollection<ToDo>(UserName).InsertOne(toDo);
        //if list exists then update else insert new
        return "Saved..";
    }


    [HttpPut]
    public String Update(ToDo todo){
         string UserName =  User.Identity.Name;
               
         _db.GetCollection<ToDo>(UserName).ReplaceOne(x => x.list == todo.list, todo);

         return "Updated";

    }



    //deleting a given list

    [HttpDelete]
    public string Delete([FromBody] JsonObject body){
        string UserName = User.Identity.Name;
        
        string list = (string)body.ElementAt(0).Value;
        // string json = System.Text.Json.JsonSerializer.Serialize(body);
        // Console.Write(json);
        _db.GetCollection<ToDo>(UserName).DeleteOne(x => x.list == list);
        return "Deleted";
    }
}