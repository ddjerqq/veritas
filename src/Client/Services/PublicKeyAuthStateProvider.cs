using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Services;

public class PublicKeyAuthStateProvider(VoterAccessor voterAccessor) : AuthenticationStateProvider
{
    private static ClaimsPrincipal EmptyPrincipal => new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var voter = await voterAccessor.GetVoterAsync();
            List<Claim> claims =
            [
                new Claim("addr", voter.Address),
                new Claim("pkey", voter.PublicKey),
                new Claim("skey", voter.PrivateKey!),
            ];

            var claimsIdentity = new ClaimsIdentity(claims, "public_key");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            return new AuthenticationState(claimsPrincipal);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return new AuthenticationState(EmptyPrincipal);
        }
    }
}