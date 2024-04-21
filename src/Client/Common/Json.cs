using System.Text.Json;
using System.Text.Json.Serialization;

namespace Client.Common;

public static class Json
{
    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        AllowTrailingCommas = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };
}