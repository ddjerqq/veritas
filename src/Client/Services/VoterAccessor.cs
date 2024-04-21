using System.Net.Http.Json;
using Application.Dto;
using Client.Common;

namespace Client.Services;

public class VoterAccessor(HttpClient http, CookieUtil cookies)
{
    public async Task<FullVoterDto> GetVoterAsync()
    {
        var address = await cookies.GetCookie(nameof(FullVoterDto.Address));
        var publicKey = await cookies.GetCookie(nameof(FullVoterDto.PublicKey));
        var privateKey = await cookies.GetCookie(nameof(FullVoterDto.PrivateKey));
        var signature = await cookies.GetCookie(nameof(FullVoterDto.Signature));

        Console.WriteLine("address: {0}", address);
        Console.WriteLine("publicKey: {0}", publicKey);
        Console.WriteLine("privateKey: {0}", privateKey);
        Console.WriteLine("signature: {0}", signature);

        FullVoterDto voter;

        if (string.IsNullOrWhiteSpace(address)
            || string.IsNullOrWhiteSpace(publicKey)
            || string.IsNullOrWhiteSpace(privateKey)
            || string.IsNullOrWhiteSpace(signature))
        {
            voter = (await http.GetFromJsonAsync<FullVoterDto>("api/v1/new_identity", Json.SerializerOptions))!;

            if (voter is null)
                throw new InvalidOperationException("voter was null");

            await cookies.SetCookie(nameof(FullVoterDto.Address), voter.Address, 365);
            await cookies.SetCookie(nameof(FullVoterDto.PublicKey), voter.PublicKey, 365);
            await cookies.SetCookie(nameof(FullVoterDto.PrivateKey), voter.PrivateKey, 365);
            await cookies.SetCookie(nameof(FullVoterDto.Signature), voter.Signature, 365);
        }
        else
        {
            voter = new FullVoterDto(address, publicKey, privateKey, signature);
        }

        return voter;
    }
}