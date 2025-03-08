namespace Plutus.Infrastructure.Enums;

[JsonConverter(typeof(ListFilterTypeConverter))]
public enum StringComparisonType
{
    DoesNotContain,
    Contains,
    Equals,
    DoesNotEqual,
    StartsWith,
    EndsWith,
    IsEmpty,
    IsNotEmpty,
    IsAnyOf
}
