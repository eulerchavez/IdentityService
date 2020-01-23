# IdentityService

## Migrations

Add-Migration IdentityContext -c ApplicationIdentityDbContext -o Migrations/IdentityContext

Add-Migration GrantContext -c PersistedGrantDbContext -o Migrations/GrantContext

Add-Migration ConfigurationContext -c ConfigurationDbContext -o Migrations/ConfigurationContext

## API

```cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = "https://localhost:5000";
                options.ApiName = "api";
                options.ApiSecret = "secret";
                options.RequireHttpsMetadata = true;
            });

        services.AddAuthorization();
    }
}
```

## MVC

```cs
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
            {
                options.Authority = "https://localhost:5000";
                options.ClientId = "client";
                options.ClientSecret = "secret";
                options.Scope.Clear();
                options.Scope.Add("openid");
                options.Scope.Add("api");
                options.SaveTokens = true;
                options.RequireHttpsMetadata = true;
                options.ResponseType = OidcConstants.ResponseTypes.IdTokenToken;
                options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

        services.AddAuthorization();
    }
}
```

```cs
public class HomeController : Controller
{
    public async Task<IActionResult> Api()
    {
        var tokenResponse = await RequestClientCredentialsTokenAsync().ConfigureAwait(false);
        var apiResponse = await GetApi(tokenResponse.AccessToken).ConfigureAwait(false);
        var viewModel = new ViewModel(tokenResponse.AccessToken, apiResponse);

        return View(viewModel);
    }

    [Authorize]
    public IActionResult Implicit()
    {
        return View();
    }

    public IActionResult Logout()
    {
        return new SignOutResult(new[]
        {
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme
        });
    }

    public async Task<IActionResult> ResourceOwnerPasswordAsync()
    {
        var tokenResponse = await RequestPasswordTokenAsync().ConfigureAwait(false);
        var apiResponse = await GetApi(tokenResponse.AccessToken).ConfigureAwait(false);
        var viewModel = new ViewModel(tokenResponse.AccessToken, apiResponse);

        return View(viewModel);
    }

    private Task<string> GetApi(string token)
    {
        var httpClient = new HttpClient();

        httpClient.SetBearerToken(token);

        return httpClient.GetStringAsync("https://localhost:5050");
    }

    private async Task<TokenResponse> RequestClientCredentialsTokenAsync()
    {
        var client = new HttpClient();

        var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5000");

        var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = "client",
            ClientSecret = "secret",
            Scope = "openid api",
        };

        return await client.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest);
    }

    private async Task<TokenResponse> RequestPasswordTokenAsync()
    {
        var client = new HttpClient();

        var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5000");

        var passwordTokenRequest = new PasswordTokenRequest
        {
            Address = disco.TokenEndpoint,
            ClientId = "client",
            ClientSecret = "secret",
            Scope = "openid api",
            UserName = "username",
            Password = "password"
        };

        return await client.RequestPasswordTokenAsync(passwordTokenRequest);
    }
}
```
