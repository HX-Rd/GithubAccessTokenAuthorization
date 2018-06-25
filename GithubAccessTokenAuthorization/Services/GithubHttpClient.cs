using HXRd.Github.GithubAccessTokenAuthorization.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HXRd.Github.GithubAccessTokenAuthorization
{
    public class GithubHttpClient
    {
        private HttpClient _client;
        private ILogger<GithubHttpClient> _logger;

        public GithubHttpClient(HttpClient client, ILogger<GithubHttpClient> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<GithubTokenInfo> GetTokenInfo(string clientId, string clientSecret, string appName, string token)
        {
            var url = $"applications/{clientId}/tokens/{token}";
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var b64client = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            request.Headers.Add("Authorization", $"Basic {b64client}");
            request.Headers.Add("User-Agent", appName);
            var result = await _client.SendAsync(request);
            if (!result.IsSuccessStatusCode)
            {
                var errorMessage = await result.Content.ReadAsStringAsync();
                throw new TokenInfoException(errorMessage);
            }
            var json = await result.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GithubTokenInfo>(json);
        }
    }
}
