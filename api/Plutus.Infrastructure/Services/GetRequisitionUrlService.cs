using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Plutus.Infrastructure.Services;

public class GetRequisitionUrlService(HttpClient httpClient, IUserInfo userInfo, IConfiguration configuration, GCAuth gcAuth, AppDbContext dbContext)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<Response> GetUrlAsync()
    {
        var existingRequisition = await dbContext.Requisitions
            .Where(x => !x.IsConfirmed)
            .FirstOrDefaultAsync(x => x.UserId == userInfo.Id);

        if (existingRequisition != null)
        {
            return new Response
            {
                Link = existingRequisition.Link
            };
        }

        var requestUri = "https://bankaccountdata.gocardless.com/api/v2/requisitions/";
        var requistionId = Guid.NewGuid().ToString();
        var agreementId = await GetAgreementIdAsync();

        var requestBody = new
        {
            redirect = $"{configuration["UiUrl"]}/requisition-confirmed?id={requistionId}",
            institution_id = "REVOLUT_REVOLT21",
            agreement = agreementId,
        };

        var jsonContent = JsonConvert.SerializeObject(requestBody);
        var response = await MakeGoCardlessRequest(jsonContent, requestUri, await gcAuth.GetAccessTokenAsync());

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var requisitionResponse = JsonConvert.DeserializeObject<RequisitionResponse>(jsonResponse);

        dbContext.Requisitions.Add(new Requisition
        {
            Id = requistionId,
            GoCardlessRequisitionId = requisitionResponse.Id,
            Created = requisitionResponse.Created,
            UserId = userInfo.Id,
            Link = requisitionResponse.Link
        });

        await dbContext.SaveChangesAsync();

        return new Response
        {
            Link = requisitionResponse.Link
        };
    }


    public async Task<string> GetAgreementIdAsync()
    {
        var requestUri = "https://bankaccountdata.gocardless.com/api/v2/agreements/enduser/";
        var requestBody = new
        {
            institution_id = "REVOLUT_REVOLT21",
            max_historical_days = 730,
            access_valid_for_days = 180,
            access_scope = new[] { "balances", "details", "transactions" }
        };

        var jsonContent = JsonConvert.SerializeObject(requestBody);
        var response = await MakeGoCardlessRequest(jsonContent, requestUri, await gcAuth.GetAccessTokenAsync());

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var responseData = JsonConvert.DeserializeObject<AggreementResponse>(jsonResponse);

        return responseData?.Id;
    }

    private async Task<HttpResponseMessage> MakeGoCardlessRequest(string jsonContent, string requestUri, string accessToken)
    {
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new StringContent(jsonContent, Encoding.UTF8, "application/json")
        };

        httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequest);
        response.EnsureSuccessStatusCode();

        return response;
    }

    public class RequisitionResponse
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public string Redirect { get; set; }
        public string Status { get; set; }
        public string InstitutionId { get; set; }
        public string Agreement { get; set; }
        public string Reference { get; set; }
        public List<object> Accounts { get; set; }
        public string Link { get; set; }
        public string Ssn { get; set; }
        public bool AccountSelection { get; set; }
        public bool RedirectImmediate { get; set; }
    }

    public class Response
    {
        public string Link { get; set; }
    }

    private class AggreementResponse
    {
        public string Id { get; set; }
    }

}

