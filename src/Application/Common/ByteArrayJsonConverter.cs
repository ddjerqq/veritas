using Domain.Common;
using Newtonsoft.Json;

namespace Application.Common;

public sealed class ByteArrayJsonConverter : JsonConverter<byte[]>
{
    public override void WriteJson(JsonWriter writer, byte[]? value, JsonSerializer serializer)
    {
        if (value is null)
        {
            writer.WriteNull();
            return;
        }

        writer.WriteValue(value.ToHexString());
    }

    public override byte[]? ReadJson(
        JsonReader reader,
        Type objectType,
        byte[]? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        var value = (string)reader.Value!;
        return value.ToBytesFromHex();
    }
}