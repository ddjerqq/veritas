using Application.Dto;

namespace Client.Services;

public class AuthHttpClientHandler(HttpMessageHandler innerHandler, CookieUtil cookies) : DelegatingHandler(innerHandler)
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Console.WriteLine("auth client start");

        var voter = await cookies.GetVoter();

        if (voter is not null)
        {
            request.Headers.Add(nameof(FullVoterDto.Address), voter.Address);
            request.Headers.Add(nameof(FullVoterDto.PublicKey), voter.PublicKey);
            request.Headers.Add(nameof(FullVoterDto.PrivateKey), voter.PrivateKey);
            request.Headers.Add(nameof(FullVoterDto.Signature), voter.Signature);
        }

        var resp = await base.SendAsync(request, cancellationToken);
        Console.WriteLine("auth client end");

        return resp;
    }
}