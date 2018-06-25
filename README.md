# GithubAccessTokenAuthorization
Github Access Token Authentication for .netcore 2.1

## What does this extension do?
This extension adds Google authorization to Web Api's in dot net 2.1. It does not do a OIDC authentication, it only verifies the github
tokens, and adds claims to the principal from github token endpoint. There are some options you can set to affect 
the behavior of the plugin and these are covered here below. There is also some caching support and will also be documented here below.

## Who is the extension for
This is by no means a best practise for providing authorization to you apis. The best way would to protect it with jwt tokens and setup
a identity server and use jwt tokens from it to authorize. You could then also sign in with google and get the github access token
and send it down to the api's if you need to access the github apis. This does introduce some overhead and some times you just
want to throw togeater an api fast and stil want some authorization. This might be a case where this extension would be helpful.
If you have anything that is not going to be just a hobby implementation of an api, go with the [Identity Server](https://github.com/IdentityServer/IdentityServer4) route.

## How to use the plugin
Using the plugin is strait forward if you are already familiar with .netcore authentication, it's setup as a authentication scheme.

Here is an example of how to use the extension.
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
    services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = GoogleAccessTokenDefaults.GOOGLE_TOKEN_SCHEME;
    })
    .AddGoogleTokenAuthorization(
        GoogleAccessTokenDefaults.GOOGLE_TOKEN_SCHEME,
        GoogleAccessTokenDefaults.GOOGLE_TOKEN_SCHEME,
        o => 
        {
            o.UseMemoryCache = true;
            o.ClientId = "YOUR-CLIENT-ID";
            o.ClientSecret "YOUR-CLIENT-SECRET";
            o.AppName = "YOUR-APP-NAME";
            o.CacheMinutes = 20;
            o.RequiredScopes = new List<string>
            {
                "repo"
            };
        }
    );
}
```
Lets go over what the options mean.
### UseMemoryCache
*Optional*   
*Default is false*   
Uses the `IMemoryCache`. It is a poor mans version of cache sinse if you have multiple servers, the cache will only work on each individual server.
If this option is set, the `IAuthenticationTicketCache` will use the `AuthenticationTicketMemoryCache` implementation wich is the only
implementation provided with the plugin. You can how ever provide you own.
### ClientId
*Required*   
*Default is null*   
This is the client id of the client that provided the access token. This is required because the token endpoint requires both ClientId and ClientSecret
### ClientSecret   
*Required*   
*Default is null*
The client seceret of the client that provided the token. This is required because the token endpoint requires both ClientId and ClientSecret
### AppName
*Required*
*Default is null*
The name of the app that provided the access token. This is required because the `User-Agent` has to be set to the App name. See
here [Github documentation](https://developer.github.com/v3/?#user-agent-required)
### RequiredScopes
*Optional*   
*Default is null*   
If this value is null, there are no scope restrictions. If you provide scopes here, the token must provide all of these scopes to be able to access the api

## Protecting an endpoint
Here is an simple example on how to protect an endpoint
```csharp
[ApiController]
public class ValuesController : ControllerBase
{
    [HttpGet]
    [Authorize(AuthenticationSchemes = GithubAccessTokenDefaults.GITHUB_TOKEN_SCHEME)]
    public ActionResult<IEnumerable<string>> Get()
    {
        return new string[] { "value1", "value2" };
    }
}
```
### Cache
The included cache `IAuthenticationTicketCache` uses this implementation `AuthenticationTicketMemoryCache` which uses `IMemoryCache`.
You can provide your own implementation of `IAuthenticationTicketCache` and bind it to the DI but to be honest if you need it,
you have propably outgrown this extension and should propably be using Identity server instead.

### Claims
There are alot of claims set on the identity with this extension. They are obtained from github's token endpoint. I will just list theme here   
Here is some documentation about the token endpoint used. [Token Endpoint](https://developer.github.com/v3/oauth_authorizations/#check-an-authorization)   
#### From Token endpont
`id`   
`url`   
`scopes`   
`token`   
`token_last_eight`   
`hashed_token`   
`note`   
`note_url`   
`updated_at`   
`created_at`   
`fingerprint`   
`app.client_id`   
`app.name`   
`app.url`   
`user.login`   
`user.id`   
`user.node_id`   
`user.avatar_url`   
`user.gravatar_id`   
`user.url`   
`user.html_url`   
`user.followers_url`   
`user.following_url`   
`user.gists_url`   
`user.starred_url`   
`user.subscriptions_url`   
`user.organizations_url`   
`user.repos_url`   
`user.events_url`   
`user.received_events_url`   
`user.type`   
`user.side_admin`   
