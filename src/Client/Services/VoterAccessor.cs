using Application.Dto;

namespace Client.Services;

public class VoterAccessor(ApiService api, CookieUtil cookies)
{
    public async Task<FullVoterDto> GetVoterAsync()
    {
        var voter = await cookies.GetVoter();
        if (voter is not null) return voter;

        voter = await api.NewIdentity();

        if (voter is null)
            throw new InvalidOperationException("voter was null");

        await cookies.SetVoter(voter);

        return voter;
    }
}