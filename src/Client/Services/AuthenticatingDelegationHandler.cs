using Application.Dto;

namespace Client.Services;

public class AuthenticatingDelegationHandler(HttpMessageHandler innerHandler, CookieUtil cookies) : DelegatingHandler(innerHandler)
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var voter = await cookies.GetVoterAsync();

        if (voter is not null)
        {
            request.Headers.Add(nameof(FullVoterDto.Address), voter.Address);
            request.Headers.Add(nameof(FullVoterDto.PublicKey), voter.PublicKey);
            request.Headers.Add(nameof(FullVoterDto.PrivateKey), voter.PrivateKey);
            request.Headers.Add(nameof(FullVoterDto.Signature), voter.Signature);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}