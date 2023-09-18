namespace BananaEngine;

using System;

using LiteDB;
using Newtonsoft.Json;

public class Resource<T> where T : class
{
    [BsonId]
    public string Name { get; set; }

    [JsonIgnore]
    public T Value { get; private set; }

    [JsonIgnore]
    [BsonIgnore]
    public bool IsValid { get { return Value != null; } }

    public Resource(string name, T resource)
    {
        Name = name;
        Value = resource;
    }

    public Resource(string name)
        : this(name, default(T))
    {
    }

    public void Hydrate(ResourceManager manager)
    {
        if (String.IsNullOrEmpty(Name))
        {
            return;
        }

        Value = manager.Get<T>(Name);
    }
}