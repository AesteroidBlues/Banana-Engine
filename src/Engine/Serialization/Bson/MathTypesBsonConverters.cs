namespace BananaEngine.Serialization;

using System;
using System.Collections.Generic;

using BananaEngine.Math;

using LiteDB;

[BsonMapperFor(typeof(Microsoft.Xna.Framework.Vector2))]
public static class XnaVector2Converter
{
    private static BsonMapper m_Mapper = null;
    public static void Register(BsonMapper dbMapper)
    {
        m_Mapper = dbMapper;
        dbMapper.RegisterType(
            serialize: XnaVector2Converter.Serialize,
            deserialize: XnaVector2Converter.Deserialize
        );
    }

    public static BsonValue Serialize(Microsoft.Xna.Framework.Vector2 vector)
    {
        return new BsonDocument()
        {
            ["X"] = (double)vector.X,
            ["Y"] = (double)vector.Y
        };
    }

    public static Microsoft.Xna.Framework.Vector2 Deserialize(BsonValue bson)
    {
        double x = bson["X"];
        double y = bson["Y"];
        return new Microsoft.Xna.Framework.Vector2((float)x, (float)y);
    }
}