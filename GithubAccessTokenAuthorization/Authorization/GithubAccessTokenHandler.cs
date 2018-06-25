using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using HXRd.Github.GithubAccessTokenAuthorization.Models;
using System.Security.Claims;

namespace HXRd.Github.GithubAccessTokenAuthorization
{
    public class GithubAccessTokenHandler : AuthenticationHandler<GithubAccessTokenOptions>
    {
        private GithubHttpClient _githubClient;
        private IOptionsMonitor<GithubAccessTokenOptions> _optionsMonitor;
        private IAuthenticationTicketCache _cache;

        public GithubAccessTokenHandler(IOptionsMonitor<GithubAccessTokenOptions> optionsMonitor, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, GithubHttpClient githubClient, IServiceProvider provider)
            : base(optionsMonitor, logger, encoder, clock)
        {
            _githubClient = githubClient;
            _optionsMonitor = optionsMonitor;
            _cache = provider.GetService<IAuthenticationTicketCache>();
        }

        protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var options = _optionsMonitor.Get(GithubAccessTokenDefaults.GITHUB_TOKEN_SCHEME);
            var headers = Request.Headers;
            var authHeader = headers.FirstOrDefault(h => h.Key == "Authorization").Value.ToString();

            if (String.IsNullOrWhiteSpace(authHeader))
            {
                return AuthenticateResult.Fail("Authentication header is null");
            }
            var token = authHeader.Split(' ')[1];
            var cacheTicket = null as AuthenticationTicket;
            if (_cache != null)
                cacheTicket = _cache.GetTicket(token);
            if (cacheTicket != null)
            {
                return AuthenticateResult.Success(cacheTicket);
            }

            var tokenInfo = null as GithubTokenInfo;
            try
            {
                tokenInfo = await _githubClient.GetTokenInfo(options.ClientId, options.ClientSecret, options.AppName, token);
                if (options.RequiredScopes != null)
                {
                    if (!(tokenInfo.Scopes.Intersect(options.RequiredScopes).Count() == options.RequiredScopes.Count()))
                    {
                        return AuthenticateResult.Fail("Token does not have all required scopes");
                    }
                }
            }
            catch (TokenInfoException ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }

            var identity = MapIdentity(tokenInfo, this.Scheme.Name);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);
            if (_cache != null)
                _cache.SetTicket(token, ticket, DateTimeOffset.Now.AddMinutes(options.CacheMinutes));
            return AuthenticateResult.Success(ticket);

        }

        private ClaimsIdentity MapIdentity(GithubTokenInfo tokenInfo, string name)
        {
            var claimList = new List<Claim>();
            if (tokenInfo.Id != null) claimList.Add(new Claim("id", tokenInfo.Id));
            if (tokenInfo.Url != null) claimList.Add(new Claim("url", tokenInfo.Url));
            if (tokenInfo.Scopes != null) claimList.Add(new Claim("scopes", string.Join(' ', tokenInfo.Scopes)));
            if (tokenInfo.Token != null) claimList.Add(new Claim("token", tokenInfo.Token));
            if (tokenInfo.TokenLastEight != null) claimList.Add(new Claim("token_last_eight", tokenInfo.TokenLastEight));
            if (tokenInfo.HashedToken != null) claimList.Add(new Claim("hashed_token", tokenInfo.HashedToken));
            if (tokenInfo.Note != null) claimList.Add(new Claim("note", tokenInfo.Note));
            if (tokenInfo.NoteUrl != null) claimList.Add(new Claim("note_url", tokenInfo.NoteUrl));
            if (tokenInfo.UpdatedAt != null) claimList.Add(new Claim("updated_at", tokenInfo.UpdatedAt));
            if (tokenInfo.CreatedAt != null) claimList.Add(new Claim("created_at", tokenInfo.CreatedAt));
            if (tokenInfo.Fingerprint != null) claimList.Add(new Claim("fingerprint", tokenInfo.Fingerprint));
            if (tokenInfo.App != null)
            {
                if (tokenInfo.App.ClientId != null) claimList.Add(new Claim("app.client_id", tokenInfo.App.ClientId));
                if (tokenInfo.App.Name != null) claimList.Add(new Claim("app.name", tokenInfo.App.Name));
                if (tokenInfo.App.Url != null) claimList.Add(new Claim("app.url", tokenInfo.App.Url));
            }
            if (tokenInfo.User != null)
            {
                if (tokenInfo.User.Login != null) claimList.Add(new Claim("user.login", tokenInfo.User.Login));
                if (tokenInfo.User.Id != null) claimList.Add(new Claim("user.id", tokenInfo.User.Id));
                if (tokenInfo.User.NodeId != null) claimList.Add(new Claim("user.node_id", tokenInfo.User.NodeId));
                if (tokenInfo.User.AvatarUrl != null) claimList.Add(new Claim("user.avatar_url", tokenInfo.User.AvatarUrl));
                if (tokenInfo.User.GravatarId != null) claimList.Add(new Claim("user.gravatar_id", tokenInfo.User.GravatarId));
                if (tokenInfo.User.Url != null) claimList.Add(new Claim("user.url", tokenInfo.User.Url));
                if (tokenInfo.User.HtmlUrl != null) claimList.Add(new Claim("user.html_url", tokenInfo.User.HtmlUrl));
                if (tokenInfo.User.FollowersUrl != null) claimList.Add(new Claim("user.followers_url", tokenInfo.User.FollowersUrl));
                if (tokenInfo.User.FollowingUrl != null) claimList.Add(new Claim("user.following_url", tokenInfo.User.FollowingUrl));
                if (tokenInfo.User.GistUrl != null) claimList.Add(new Claim("user.gists_url", tokenInfo.User.GistUrl));
                if (tokenInfo.User.StarredUrl != null) claimList.Add(new Claim("user.starred_url", tokenInfo.User.StarredUrl));
                if (tokenInfo.User.SubscriptionsUrl != null) claimList.Add(new Claim("user.subscriptions_url", tokenInfo.User.SubscriptionsUrl));
                if (tokenInfo.User.OrganizationsUrl != null) claimList.Add(new Claim("user.organizations_url", tokenInfo.User.OrganizationsUrl));
                if (tokenInfo.User.ReposUrl != null) claimList.Add(new Claim("user.repos_url", tokenInfo.User.ReposUrl));
                if (tokenInfo.User.EventsUrl != null) claimList.Add(new Claim("user.events_url", tokenInfo.User.EventsUrl));
                if (tokenInfo.User.ReceivedEventsUrl != null) claimList.Add(new Claim("user.received_events_url", tokenInfo.User.ReceivedEventsUrl));
                if (tokenInfo.User.Type != null) claimList.Add(new Claim("user.type", tokenInfo.User.Type));
                if (tokenInfo.User.SiteAdmin != null) claimList.Add(new Claim("user.side_admin", tokenInfo.User.SiteAdmin.ToString()));
            }
            return new ClaimsIdentity(claimList, name);
        }
    }
}
