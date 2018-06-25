using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Github.GithubAccessTokenAuthorization
{
    public class AuthenticationTicketMemoryCache : IAuthenticationTicketCache
    {
        private IMemoryCache _memCache;

        public AuthenticationTicketMemoryCache(IMemoryCache memCache)
        {
            _memCache = memCache;
        }

        public AuthenticationTicket GetTicket(object key) => _memCache.Get<AuthenticationTicket>(key);

        public AuthenticationTicket SetTicket(object key, AuthenticationTicket ticket, DateTimeOffset expires) => _memCache.Set(key, ticket, expires);
    }
}
