using System.Security.Claims;
using System.Text.Encodings.Web;
using Domain.Common;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Presentation.Auth;

public class PublicKeyBearerSchemeOptions : AuthenticationSchemeOptions;

public class PublicKeyBearerAuthHandler(
    IOptionsMonitor<PublicKeyBearerSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<PublicKeyBearerSchemeOptions>(options, logger, encoder)
{
    public const string SchemaName = "public_key";

    public const string PubKeyHeaderName = "X-Public-Key";
    public const string SignatureHeaderName = "X-Signature";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // TODO test if it is possible to impersonate.

        var pKey = ExtractPublicKey(Request);
        var sig = ExtractSignature(Request);

        if (pKey is null || sig is null)
            return Task.FromResult(AuthenticateResult.Fail("no public key or signature provided"));

        var voter = Voter.FromPubKey(pKey.ToBytesFromHex());
        var signatureValid = voter.Verify(voter.Address.ToBytesFromHex(), sig.ToBytesFromHex());
        if (!signatureValid)
            return Task.FromResult(AuthenticateResult.Fail("invalid signature"));

        List<Claim> claims =
        [
            new Claim("addr", voter.Address),
            new Claim("pub_key", pKey),
        ];
        var claimsIdentity = new ClaimsIdentity(claims, SchemaName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(claimsIdentity), SchemaName);

        Context.Items[nameof(Voter)] = voter;
        Context.User = new ClaimsPrincipal(claimsIdentity);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private string? ExtractPublicKey(HttpRequest request)
    {
        Request.Headers.TryGetValue(PubKeyHeaderName, out var pKeyHeader);
        Request.Cookies.TryGetValue(PubKeyHeaderName, out var pKeyCookie);
        var pKey = (string?)pKeyHeader ?? pKeyCookie;
        return ValidateHexString(pKey) ? pKey : null;
    }

    private string? ExtractSignature(HttpRequest request)
    {
        Request.Headers.TryGetValue(SignatureHeaderName, out var sigHeader);
        Request.Cookies.TryGetValue(SignatureHeaderName, out var sigCookie);
        var sig = (string?)sigHeader ?? sigCookie;
        return ValidateHexString(sig) ? sig : null;
    }

    private bool ValidateHexString(string? hexString) =>
        !string.IsNullOrWhiteSpace(hexString)
        && hexString.Length < 512
        && hexString.All(char.IsAsciiHexDigit);
}