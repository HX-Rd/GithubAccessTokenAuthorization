using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Github.GithubAccessTokenAuthorization
{
    public static class AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddGithubTokenAuthorization(this AuthenticationBuilder builder)
        {
            return AddGithubTokenAuthorization(builder, GithubAccessTokenDefaults.GITHUB_TOKEN_SCHEME, null, o => { });
        }
        public static AuthenticationBuilder AddGithubTokenAuthorization(this AuthenticationBuilder builder, Action<GithubAccessTokenOptions> configurationOptions)
        {
            return AddGithubTokenAuthorization(builder, GithubAccessTokenDefaults.GITHUB_TOKEN_SCHEME, null, configurationOptions);
        }

        public static AuthenticationBuilder AddGithubTokenAuthorization(this AuthenticationBuilder builder, string authenticationScheme)
        {
            return AddGithubTokenAuthorization(builder, authenticationScheme, null, o => { });
        }

        public static AuthenticationBuilder AddGithubTokenAuthorization(this AuthenticationBuilder builder, string authenticationScheme, Action<GithubAccessTokenOptions> configureOptions)
        {
            return AddGithubTokenAuthorization(builder, authenticationScheme, null, configureOptions);
        }

        public static AuthenticationBuilder AddGithubTokenAuthorization(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GithubAccessTokenOptions> configureOptions)
        {
            builder.Services.AddHttpClient<GithubHttpClient>(client => client.BaseAddress = new Uri("https://api.github.com"));
            var @return = null as AuthenticationBuilder;
            if (displayName == null)
                @return = builder.AddScheme<GithubAccessTokenOptions, GithubAccessTokenHandler>(authenticationScheme, configureOptions);
            @return = builder.AddScheme<GithubAccessTokenOptions, GithubAccessTokenHandler>(authenticationScheme, displayName, configureOptions);
            var provider = builder.Services.BuildServiceProvider();
            var options = provider.GetService<IOptionsMonitor<GithubAccessTokenOptions>>().Get(GithubAccessTokenDefaults.GITHUB_TOKEN_SCHEME);
            if (options.UseMemoryCache)
                builder.Services.AddSingleton<IAuthenticationTicketCache, AuthenticationTicketMemoryCache>();
            return @return;
        }
    }
}
