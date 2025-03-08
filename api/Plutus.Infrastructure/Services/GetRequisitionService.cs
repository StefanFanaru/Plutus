using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Plutus.Infrastructure.Services;

public class GetRequisitionUrlService(HttpClient httpClient, IUserInfo userInfo, IConfiguration configuration, GCAuth gcAuth, AppDbContext dbContext)
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<Response> GetUrlAsync()
    {
        var requestUri = "https://bankaccountdata.gocardless.com/api/v2/requisitions/";
        var requistionId = Guid.NewGuid().ToString();
        var requestBody = new
        {
            redirect = $"{configuration["UiUrl"]}/requisition-confirmed?id={requistionId}",
            institution_id = "REVOLUT_REVOLT21",
            agreement = configuration["Secrets:GoCardless:AgreementId"],
        };

        var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
        var httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json")
        };

        var accessToken = await gcAuth.GetAccessTokenAsync();
        httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(httpRequestMessage);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var requisitionResponse = JsonConvert.DeserializeObject<RequisitionResponse>(jsonResponse);

        var existingRequisition = await dbContext.Requisitions.FirstOrDefaultAsync(x => x.UserId == userInfo.Id);

        if (existingRequisition != null)
        {
            dbContext.Requisitions.Remove(existingRequisition);
        }

        dbContext.Requisitions.Add(new Requisition
        {
            Id = requistionId,
            GoCardlessRequisitionId = requisitionResponse.Id,
            Created = requisitionResponse.Created,
            UserId = userInfo.Id,
        });

        await dbContext.SaveChangesAsync();

        return new Response
        {
            Link = requisitionResponse.Link
        };
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

}

