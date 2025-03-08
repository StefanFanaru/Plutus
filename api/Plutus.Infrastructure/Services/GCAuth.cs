using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Plutus.Infrastructure.Services;

public class GCAuth(IHttpClientFactory httpClientFactory, IMemoryCache cache, IConfiguration configuration)
{
    private const string TokensCacheKey = "GO_CARDLESS_TOKENS";

    public async Task<string> GetAccessTokenAsync()
    {
        if (cache.TryGetValue<GCTokens>(TokensCacheKey, out var tokensCacheValue))
        {
            return tokensCacheValue!.Access;
        }

        var tokens = await RequestTokens();
        cache.Set(TokensCacheKey, tokens, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(86000)
        });


        return tokens.Access;
    }


    private async Task<GCTokens> RequestTokens()
    {
        var secretId = configuration["Secrets:GoCardless:SecretId"];
        var secretPassword = configuration["Secrets:GoCardless:SecretPassword"];
        var request = new HttpRequestMessage(HttpMethod.Post, "https://bankaccountdata.gocardless.com/api/v2/token/new/");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(JsonConvert.SerializeObject(new { secret_id = secretId, secret_key = secretPassword }), Encoding.UTF8, "application/json");
        var client = httpClientFactory.CreateClient();
        var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to get GoCardless tokens");
        }
        var responseContent = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<GCTokens>(responseContent)!;
    }
}
