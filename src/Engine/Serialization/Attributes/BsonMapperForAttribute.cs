namespace BananaEngine.Serialization;

using System;

public class BsonMapperForAttribute : Attribute
{
    public Type ForType { get; set; }

    public BsonMapperForAttribute(Type type)
    {
        ForType = type;
    }
}