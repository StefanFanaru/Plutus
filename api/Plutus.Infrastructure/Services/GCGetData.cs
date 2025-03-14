namespace Plutus.Infrastructure.Services;

public class GCGetData(IHttpClientFactory httpClientFactory, GCAuth auth)
{
    public async Task<GoCardlessTransactionsReponse> GetTransactionsAsync(int daysBack, string accountId)
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + await auth.GetAccessTokenAsync());
        var date_from = DateTime.UtcNow.AddDays(-daysBack);
        var date_to = DateTime.UtcNow;
        var response = await client.GetAsync($"https://bankaccountdata.gocardless.com/api/v2/accounts/{accountId}/transactions/?date_from={date_from:yyyy-MM-dd}&date_to={date_to:yyyy-MM-dd}");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to get transactions from GoCardless - Status code {response.StatusCode}");
        }
        var content = await response.Content.ReadAsStringAsync();

        var transactions = JsonConvert.DeserializeObject<GoCardlessTransactionsReponse>(content);
        return transactions;
    }

    public async Task<decimal> GetBalanceAsync(string accountId)
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + await auth.GetAccessTokenAsync());
        var response = await client.GetAsync($"https://bankaccountdata.gocardless.com/api/v2/accounts/{accountId}/balances/");
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to get balance from GoCardless - Status code {response.StatusCode}");
        }
        var content = await response.Content.ReadAsStringAsync();
        // var content = "{\"balances\": [{\"balanceAmount\": {\"amount\": \"100.00\", \"currency\": \"RON\"}}]}";
        var balance = JsonConvert.DeserializeObject<GCBalance>(content);
        return decimal.Parse(balance!.Balances[0].BalanceAmount.Amount);
    }

    public class GoCardlessTransactionsReponse
    {
        [JsonProperty("transactions")]
        public GCTransactions Transactions { get; set; }
    }
}
