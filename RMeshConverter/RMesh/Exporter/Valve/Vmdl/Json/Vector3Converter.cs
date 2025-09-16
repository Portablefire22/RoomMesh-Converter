using System.Text.Json;
using System.Text.Json.Serialization;

namespace RMeshConverter.Exporter.Valve.Json;

public class Vector3Converter : JsonConverter<Vector3Model>
{
    public override Vector3Model? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, Vector3Model value, JsonSerializerOptions options)
    {
        var str = $"{value.X},{value.Y},{value.Z}";
        writer.WriteStringValue(str);
    }
}