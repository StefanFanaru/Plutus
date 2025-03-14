using System.Net.Http.Headers;
using Plutus.Infrastructure.Business.Transactions;
using Serilog;

namespace Plutus.Infrastructure.Services;

public class GCListAccounts(HttpClient httpClient, IUserInfo userInfo, GCAuth gcAuth, AppDbContext dbContext, GCGetData gcGetData)
{
    public async Task<List<string>> GetAccountIdsAsync()
    {
        var requisitionId = (await dbContext.Requisitions.SingleAsync(x => x.UserId == userInfo.Id)).GoCardlessRequisitionId;
        var requestUri = $"https://bankaccountdata.gocardless.com/api/v2/requisitions/{requisitionId}/";

        var accessToken = await gcAuth.GetAccessTokenAsync();

        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var requisitionResponse = JsonConvert.DeserializeObject<RequisitionDetailsResponse>(jsonResponse);

        return requisitionResponse.Accounts;
    }

    public async Task<Response> ListAsync()
    {
        var accountIds = await GetAccountIdsAsync();
        Log.Information("AccountIds: {AccountIds}", string.Join(", ", accountIds));
        return new Response
        {
            Accounts = await GetAccountDetailsAsync(accountIds)
        };
    }

    private async Task<List<AccountResponseItem>> GetAccountDetailsAsync(List<string> accountIds)
    {
        var accountDetails = new List<AccountResponseItem>();

        foreach (var accountId in accountIds)
        {
            var requestUri = $"https://bankaccountdata.gocardless.com/api/v2/accounts/{accountId}/details/";
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var accessToken = await gcAuth.GetAccessTokenAsync();
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.SendAsync(httpRequestMessage);
            response.EnsureSuccessStatusCode();


            var jsonResponse = await response.Content.ReadAsStringAsync();
            Log.Information("Response: {Response}", jsonResponse);
            // var jsonResponse = "{\"account\": {\"resourceId\": \"24446eef-4782-4b09-870e-6f5cjb21fdd3\", \"iban\": \"DKJF3490341290898\", \"currency\": \"RON\", \"ownerName\": \"GIGI MUSCHI\"}}";
            var accountResponse = JsonConvert.DeserializeObject<AccountDetailsResponse>(jsonResponse).Account;

            var accountItem = new AccountResponseItem
            {
                AccountId = accountId,
                Iban = accountResponse.Iban,
                Currency = accountResponse.Currency,
                Transactions = await GetTransactionsSample(accountId)
            };
            accountDetails.Add(accountItem);
        }

        return accountDetails;
    }

    private async Task<List<TransactionListItem>> GetTransactionsSample(string accountId)
    {
        var transactions = await gcGetData.GetTransactionsAsync(30, accountId);

        if (transactions.Transactions == null || transactions.Transactions.Booked.Count == 0)
        {
            return [];
        }

        var transactionList = new List<TransactionListItem>();
        var bookedTransactions = GCInsertData.CleanTransactions(transactions);
        foreach (var transaction in bookedTransactions)
        {
            transactionList.Add(new TransactionListItem
            {
                Id = transaction.TransactionId,
                BookedAt = transaction.BookingDateTime,
                Amount = transaction.TransactionAmount.Amount,
                Type = transaction.GetTransactionType(),
                ObligorName = transaction.TransactionAmount.Amount < 0 ? transaction.CreditorName : transaction.DebtorName,
            });
        }

        return [.. transactionList.OrderByDescending(x => x.BookedAt).Take(5)];
    }

    public class RequisitionDetailsResponse
    {
        public List<string> Accounts { get; set; }
    }

    public class AccountDetailsResponse
    {
        public AccountDetailsReponseItem Account { get; set; }
        public class AccountDetailsReponseItem
        {
            public string Iban { get; set; }
            public string Currency { get; set; }
        }
    }

    public class AccountResponseItem
    {
        public string AccountId { get; set; }
        public string Iban { get; set; }
        public string Currency { get; set; }
        public List<TransactionListItem> Transactions { get; set; }
    }

    public class Response
    {
        public List<AccountResponseItem> Accounts { get; set; }
    }

}

