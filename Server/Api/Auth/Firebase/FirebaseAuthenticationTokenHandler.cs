using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Server.Authentication;

public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly FirebaseApp _firebseApp;

    public FirebaseAuthenticationHandler
        (IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        FirebaseApp firebaseApp)
        : base(options, logger, encoder, clock)
    {
        _firebseApp = firebaseApp;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = "Authorization";
        var containsAuthorization = Context.Request.Headers.ContainsKey(authHeader);

        if (!containsAuthorization)
        {
            return AuthenticateResult.NoResult();
        }

        var bearerToken = Context.Request.Headers[authHeader].ToString();

        if (!bearerToken.StartsWith("Bearer "))
        {
            return AuthenticateResult.Fail("Invalid scheme.");
        }

        var token = bearerToken.Substring("Bearer ".Length);

        try
        {
            var firebaseToken = await FirebaseAuth.GetAuth(_firebseApp).VerifyIdTokenAsync(token);
            var claimsPrincipal = new ClaimsPrincipal(new List<ClaimsIdentity>
            {
            new ClaimsIdentity(ToClaims(firebaseToken.Claims), nameof(FirebaseAuthenticationHandler))
            });
            var authenticationTicket = new AuthenticationTicket(claimsPrincipal, JwtBearerDefaults.AuthenticationScheme);

            return AuthenticateResult.Success(authenticationTicket);
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail(ex);
        }
    }

    private IEnumerable<Claim>? ToClaims(IReadOnlyDictionary<string, object> claims)
    {
        return new List<Claim>
        {
            new Claim("id", claims["user_id"].ToString()),
            new Claim("email", claims["email"].ToString())
        };
    }
}