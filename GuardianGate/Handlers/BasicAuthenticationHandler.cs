using System;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GuardianGate.Handlers;


public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IConfiguration _config;
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IConfiguration config)
        : base(options, logger, encoder, clock)
    {
        _config = config;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.Fail("Missing Authorization Header");

        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            var username = credentials[0];
            var password = credentials[1];

            if (ValidBasicToken(username, password))
            {
                var claims = new[] { new Claim(ClaimTypes.Name, username) };
                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            else
            {
                return AuthenticateResult.Fail("Invalid username or password");
            }
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail("Error: " + ex.Message);
        }
    }
    
    //Valid Basic Token
    public bool ValidBasicToken(string username, string password)
    {
        return username == (Environment.GetEnvironmentVariable("BASIC_USERNAME") ?? _config["Basic:Username"]) && password == (Environment.GetEnvironmentVariable("BASIC_PASSWORD") ?? _config["Basic:Password"]);
    }
    
}
