namespace Plutus.Infrastructure.Common;

public class ListFilterTypeConverter : JsonConverter<StringComparisonType>
{
    public override StringComparisonType ReadJson(JsonReader reader, Type objectType, StringComparisonType existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var value = reader.Value.ToString();
        return value switch
        {
            "doesNotContain" => StringComparisonType.DoesNotContain,
            "contains" => StringComparisonType.Contains,
            "equals" => StringComparisonType.Equals,
            "doesNotEqual" => StringComparisonType.DoesNotEqual,
            "startsWith" => StringComparisonType.StartsWith,
            "endsWith" => StringComparisonType.EndsWith,
            "isEmpty" => StringComparisonType.IsEmpty,
            "isNotEmpty" => StringComparisonType.IsNotEmpty,
            "isAnyOf" => StringComparisonType.IsAnyOf,
            _ => throw new JsonException($"Unknown value: {value}")
        };
    }

    public override void WriteJson(JsonWriter writer, StringComparisonType value, JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString().ToLowerInvariant());
    }
}
