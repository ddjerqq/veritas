using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace Client.Services;

public class PublicKeyAuthStateProvider(HttpClient http) : AuthenticationStateProvider
{
    private static ClaimsPrincipal EmptyPrincipal => new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // TODO this will be purely read from localstorage

            // var claims = body
            //     .Select(kv => new Claim(kv.Key, kv.Value))
            //     .ToList();

            var claimsIdentity = new ClaimsIdentity([], "bearer");
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