using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Plutus.Infrastructure.Business;
using Plutus.Infrastructure.Enums;

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

