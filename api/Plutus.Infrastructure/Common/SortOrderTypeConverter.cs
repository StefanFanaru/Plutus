using Newtonsoft.Json;
using Plutus.Infrastructure.Enums;

namespace Plutus.Infrastructure.Common;

public class SortOrderTypeConverter : JsonConverter<SortOrderType>
{
    public override SortOrderType ReadJson(JsonReader reader, Type objectType, SortOrderType existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var value = reader.Value.ToString();
        return value switch
        {
            "asc" => SortOrderType.Asc,
            "desc" => SortOrderType.Desc,
            _ => throw new JsonException($"Unknown value: {value}")
        };
    }

    public override void WriteJson(JsonWriter writer, SortOrderType value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString().ToLowerInvariant());
    }
}
