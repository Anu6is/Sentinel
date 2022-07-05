using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace Sentinel;

public class SentrySignatureAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string SentrySignatureKey = "sentry-hook-signature";
    private readonly byte[] ClientSecret;

    public SentrySignatureAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock) 
    {
        ClientSecret = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable(SentrySignatureKey)!);
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(SentrySignatureKey, out var sentrySignature)) return AuthenticateResult.NoResult();
        if (string.IsNullOrEmpty(sentrySignature)) return AuthenticateResult.NoResult();
        if (!await ValidateSignatureAsync(sentrySignature)) return AuthenticateResult.Fail("Invalid Authorization Signature");

        var claims = new[] { new Claim(ClaimTypes.Name, "SentryHook") };
        var identity = new ClaimsIdentity(claims, "SentrySignature");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
    }

    private async Task<bool> ValidateSignatureAsync(StringValues sentrySignature)
    {
        using var hmac = new HMACSHA256(ClientSecret);
        using var stream = new StreamReader(Request.Body);

        var jsonBody = await stream.ReadToEndAsync();
        var signature = hmac.ComputeHash(Encoding.UTF8.GetBytes(jsonBody))
                            .Aggregate(new StringBuilder(), (builder, @byte) => builder.Append(@byte.ToString("x2"))).ToString();

        return signature.Equals(sentrySignature);
    }
}