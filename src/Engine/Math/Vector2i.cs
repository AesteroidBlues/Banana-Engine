namespace BananaEngine.Util;

using System;
using Newtonsoft.Json;

[JsonConverter(typeof(Vector2iConverter))]
public class Vector2i
{
    public static Vector2i Zero { get { return new Vector2i(0); } }

    public int X { get; set; }
    public int Y { get; set; }

    public Vector2i(int x, int y)
    {
        X = x;
        Y = y;
    }

    public Vector2i(int v)
    {
        X = v;
        Y = v;
    }
}

public class Vector2iConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector2i);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var arr = serializer.Deserialize<int[]>(reader);
        return new Vector2i(arr[0], arr[1]);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var v = (Vector2i)value;
        serializer.Serialize(writer, new[] { v.X, v.Y });
    }
}