using System.Text.Json;
using System.Text.Json.Serialization;

namespace RMeshConverter.Exporter.Valve.Json;

public class Vector4Converter : JsonConverter<Vector4Model>
{
    
    public override void Write(Utf8JsonWriter writer, Vector4Model value, JsonSerializerOptions options)
    {
        var str = $"{value.X},{value.Y},{value.Z},{value.W}";
        writer.WriteStringValue(str);
    }

    public override Vector4Model? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}