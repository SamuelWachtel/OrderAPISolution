using Serilog;
using System.Net.Http.Json;

namespace OrderApi
{
    public class SecurityToken
    {
        public async Task<string> GetAccessToken()
        {
            var config = ProgramConfiguration.LoadConfig();
            using (var client = new HttpClient())
            {
                var parameters = new
                {
                    client_id = config.username,
                    client_secret = config.password,
                    grant_type = "client_credentials"
                };

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("client_id", config.username),
                    new KeyValuePair<string, string>("client_secret", config.password),
                    new KeyValuePair<string, string>("grant_type", "client_credentials")
                });
                Log.Information($"[Validation] Requesting access token.");

                try
                {
                    HttpResponseMessage response = await client.PostAsync(config.identityService, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                        string accessToken = tokenResponse.access_token;
                        Log.Information($"[Validation] Access token obtained successfully.");
                        Log.Logger.LogLine();
                        return accessToken;
                    }
                    else
                        throw new Exception("Failed to obtain access token. Status code: " + response.StatusCode);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Failed to obtain access token.");
                    throw;
                }
            }
        }
    }

    public class TokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
    }
}