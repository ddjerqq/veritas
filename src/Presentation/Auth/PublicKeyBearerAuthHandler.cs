using System.Security.Claims;
using System.Text.Encodings.Web;
using Application.Dto;
using Domain.Entities;
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

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var address = Extract(nameof(FullVoterDto.Address));
        var publicKey = Extract(nameof(FullVoterDto.PublicKey));
        var privateKey = Extract(nameof(FullVoterDto.PrivateKey));
        var signature = Extract(nameof(FullVoterDto.Signature));

        if (address is null || publicKey is null || privateKey is null || signature is null)
            return Task.FromResult(AuthenticateResult.Fail("no voter info provided"));

        var voter = Voter.FromKeyPair(publicKey, privateKey);
        if (!voter.VerifyAddressSignature(signature))
            return Task.FromResult(AuthenticateResult.Fail("invalid signature"));

        Context.Items[nameof(Voter)] = voter;

        List<Claim> claims =
        [
            new Claim("addr", voter.Address),
            new Claim("pkey", voter.PublicKey),
            new Claim("skey", voter.PrivateKey!),
        ];
        var claimsIdentity = new ClaimsIdentity(claims, SchemaName);
        Context.User = new ClaimsPrincipal(claimsIdentity);

        return Task.FromResult(
            AuthenticateResult.Success(
                new AuthenticationTicket(Context.User, SchemaName)));
    }

    private string? Extract(string key)
    {
        Request.Query.TryGetValue(key, out var query);
        Request.Headers.TryGetValue(key, out var header);
        Request.Cookies.TryGetValue(key, out var cookie);

        var value = (string?)query ?? (string?)header ?? cookie;

        var isValid =
            !string.IsNullOrWhiteSpace(value)
            && value.Length <= 128
            && (value.All(char.IsAsciiHexDigit) || value.StartsWith("0x"));

        return isValid ? value : null;
    }
}