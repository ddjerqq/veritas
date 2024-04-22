using System.Net.Http.Json;
using Application.Dto;
using Client.Common;

namespace Client.Services;

public class VoterAccessor(HttpClient http, CookieUtil cookies)
{
    public async Task<FullVoterDto> GetVoterAsync()
    {
        var voter = await cookies.GetVoter();

        if (voter is null)
        {
            voter = (await http.GetFromJsonAsync<FullVoterDto>("api/v1/new_identity", Json.SerializerOptions))!;

            if (voter is null)
                throw new InvalidOperationException("voter was null");

            await cookies.SetVoter(voter);
        }

        return voter;
    }
}