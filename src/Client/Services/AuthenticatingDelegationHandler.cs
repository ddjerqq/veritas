using Application.Dto;

namespace Client.Services;

public class AuthenticatingDelegationHandler(HttpMessageHandler innerHandler, IpService ipService, CookieUtil cookies)
    : DelegatingHandler(innerHandler)
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var voter = await cookies.GetVoterAsync();
        // var ip = await ipService.GetIpAddress(ct);

        if (voter is not null)
        {
            request.Headers.Add(nameof(FullVoterDto.Address), voter.Address);
            request.Headers.Add(nameof(FullVoterDto.PublicKey), voter.PublicKey);
            request.Headers.Add(nameof(FullVoterDto.PrivateKey), voter.PrivateKey);
            request.Headers.Add(nameof(FullVoterDto.Signature), voter.Signature);
            // request.Headers.Add("X-Real-IP", ip);
        }

        return await base.SendAsync(request, ct);
    }
}