namespace Plutus.Infrastructure.Dtos;

public class GCTokens
{
    public required string Access { get; set; }
    [JsonProperty("access_expires")]
    public required int AccessExpires { get; set; }
    public required string Refresh { get; set; }
}
