using Newtonsoft.Json;

namespace Plutus.Infrastructure.Dtos
{
    public class GCBalance
    {
        [JsonProperty("balances")]
        public List<Balance> Balances { get; set; }
    }

    public class Balance
    {
        [JsonProperty("balanceAmount")]
        public BalanceAmount BalanceAmount { get; set; }

        [JsonProperty("balanceType")]
        public string BalanceType { get; set; }

        [JsonProperty("referenceDate")]
        public string ReferenceDate { get; set; }
    }

    public class BalanceAmount
    {
        [JsonProperty("amount")]
        public string Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }
    }
}
