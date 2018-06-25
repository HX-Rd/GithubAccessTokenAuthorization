using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Github.GithubAccessTokenAuthorization
{
    public class GithubAccessTokenOptions : AuthenticationSchemeOptions
    {
        public bool UseMemoryCache { get; set; } = false;
        public string AppName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public int CacheMinutes { get; set; } = 20;
        public List<string> RequiredScopes { get; set; }
    }
}
