using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Plutus.Infrastructure.Dtos;

namespace Plutus.Infrastructure.Services
{
    public class GCGetData(IHttpClientFactory httpClientFactory, GCAuth auth, IConfiguration configuration)
    {
        public async Task<GoCardlessTransactionsReponse> GetTransactionsAsync()
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + await auth.GetAccessTokenAsync());
            var date_from = DateTime.UtcNow.AddDays(-30);
            var date_to = DateTime.UtcNow;
            var response = await client.GetAsync($"https://bankaccountdata.gocardless.com/api/v2/accounts/{configuration["Secrets:GoCardless:RevolutAccountID"]}/transactions/?date_from={date_from:yyyy-MM-dd}&date_to={date_to:yyyy-MM-dd}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get transactions from GoCardless - Status code {response.StatusCode}");
            }
            var content = await response.Content.ReadAsStringAsync();
            var transactions = JsonConvert.DeserializeObject<GoCardlessTransactionsReponse>(content);
            return transactions;
        }

        public async Task<decimal> GetBalanceAsync()
        {
            var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + await auth.GetAccessTokenAsync());
            var response = await client.GetAsync($"https://bankaccountdata.gocardless.com/api/v2/accounts/{configuration["Secrets:GoCardless:RevolutAccountID"]}/balances/");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to get balance from GoCardless - Status code {response.StatusCode}");
            }
            var content = await response.Content.ReadAsStringAsync();
            var balance = JsonConvert.DeserializeObject<GCBalance>(content);
            return decimal.Parse(balance!.Balances.First().BalanceAmount.Amount);
        }

        public class GoCardlessTransactionsReponse
        {
            [JsonProperty("transactions")]
            public GCTransactions Transactions { get; set; }
        }
    }


}
