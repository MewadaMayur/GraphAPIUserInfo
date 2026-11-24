
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace GraphAPIUserInfo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GraphController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public GraphController(IHttpClientFactory httpClientFactory,IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
        {
            // 1. Read Entra ID configuration
            var tenantId = _configuration["AzureAd:TenantId"];
            var clientId = _configuration["AzureAd:ClientId"];
            var clientSecret = _configuration["AzureAd:ClientSecret"];

            if (string.IsNullOrWhiteSpace(tenantId) || string.IsNullOrWhiteSpace(clientId) ||
                string.IsNullOrWhiteSpace(clientSecret))
            {
                return StatusCode(500, "Azure AD configuration is missing.");
            }

            var httpClient = _httpClientFactory.CreateClient();

            // 2. Request app-only access token
            var tokenUrl =$"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

            var tokenParameters = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["scope"] = "https://graph.microsoft.com/.default",
                ["grant_type"] = "client_credentials"
            };

            using var tokenResponse = await httpClient.PostAsync(tokenUrl,new FormUrlEncodedContent(tokenParameters),cancellationToken);

            var tokenJson = await tokenResponse.Content.ReadAsStringAsync(cancellationToken);

            if (!tokenResponse.IsSuccessStatusCode)
            {
                return new ContentResult
                {
                    StatusCode = (int)tokenResponse.StatusCode,
                    ContentType = "application/json",
                    Content = tokenJson
                };
            }

            using var tokenDocument = JsonDocument.Parse(tokenJson);

            var accessToken = tokenDocument.RootElement
                .GetProperty("access_token")
                .GetString();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return StatusCode(500, "Access token was not returned.");
            }

            // 3. Call Microsoft Graph API
            using var graphRequest = new HttpRequestMessage(HttpMethod.Get,"https://graph.microsoft.com/v1.0/users");

            graphRequest.Headers.Authorization =new AuthenticationHeaderValue("Bearer", accessToken);

            // 4. Get users
            using var graphResponse = await httpClient.SendAsync(graphRequest,cancellationToken);

            var graphJson = await graphResponse.Content.ReadAsStringAsync(cancellationToken);

            // 5. Return Graph API response
            return new ContentResult
            {
                StatusCode = (int)graphResponse.StatusCode,
                ContentType = "application/json",
                Content = graphJson
            };
        }
    }
}
