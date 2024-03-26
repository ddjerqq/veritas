using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Domain.Common;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Presentation.Auth;

public class PublicKeyBearerSchemeOptions : AuthenticationSchemeOptions;

// TODO everyone can imitate another user, because they can take their public key.
// however only the holder of the private key can sign the data, everyone can verify
// the data using the public key. so what we need to do is, alongside sending the public key,
// sign client's address, and send it to the server.

public class PublicKeyBearerAuthHandler(
    IOptionsMonitor<PublicKeyBearerSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<PublicKeyBearerSchemeOptions>(options, logger, encoder)
{
    public const string SchemaName = "public_key";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var pKey = Request.Headers.Authorization.ToString();

        if (string.IsNullOrWhiteSpace(pKey))
            pKey = Request.Cookies
                .FirstOrDefault(c => string.Equals(c.Key, HeaderNames.Authorization, StringComparison.InvariantCultureIgnoreCase))
                .Value;

        if (string.IsNullOrWhiteSpace(pKey))
            return Task.FromResult(AuthenticateResult.Fail("no public key"));

        if (!pKey.All(char.IsAsciiHexDigit) || pKey.Length > 512)
            return Task.FromResult(AuthenticateResult.Fail("bad public key"));

        var voter = Voter.FromPubKey(pKey.ToBytesFromHex());

        List<Claim> claims = [
            new Claim("addr", voter.Address),
            new Claim("pub_key", pKey),
        ];
        var claimsIdentity = new ClaimsIdentity(claims, SchemaName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), SchemaName);

        Context.Items[nameof(Voter)] = voter;
        Context.User = new ClaimsPrincipal(claimsIdentity);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}