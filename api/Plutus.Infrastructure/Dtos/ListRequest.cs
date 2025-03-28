using System.ComponentModel;
using Newtonsoft.Json.Converters;

namespace Plutus.Infrastructure.Dtos;

public class ListRequest
{
    public int PageNumber { get; set; }
    [DefaultValue(10)]
    public int PageSize { get; set; }
    [DefaultValue("")]
    public string SortField { get; set; }
    [JsonConverter(typeof(StringEnumConverter))]
    public SortOrderType SortOrder { get; set; }
    public ListRequestFilter Filter { get; set; }


    public class ListRequestFilter
    {
        [DefaultValue("")]
        public string Field { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public StringComparisonType Operator { get; set; }
        [DefaultValue("")]
        public string Value { get; set; }
    }
}
