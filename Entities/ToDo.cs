

using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class ToDo{
    public string? list {get;set;}
    public string[]? complete {get;set;}

    public string[]? incomplete {get;set;}

}