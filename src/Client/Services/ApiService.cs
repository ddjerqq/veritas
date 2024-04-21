using System.Net.Http.Json;
using Application.Dto;
using Client.Common;
using Domain.ValueObjects;

namespace Client.Services;

public class ApiService(HttpClient http, VoteService voteService)
{
    public async Task<FullVoterDto?> NewIdentity() =>
        await http.GetFromJsonAsync<FullVoterDto>("api/v1/new_identity", Json.SerializerOptions);

    public async Task<Dictionary<string, string>?> GetClaims() =>
        await http.GetFromJsonAsync<Dictionary<string, string>>("api/v1/claims");

    public async Task CastVote(int partyId)
    {
        var castVoteCommand = await voteService.CreateVoteCommand(partyId);
        await http.PostAsJsonAsync("api/v1/cast_vote", castVoteCommand, Json.SerializerOptions);
    }

    public async Task<Dictionary<Party, int>?> GetPartyVoteCounts() =>
        await http.GetFromJsonAsync<Dictionary<Party, int>>("api/v1/stats/counts");

    public async Task<Dictionary<Party, Dictionary<DateOnly, int>>?> GetPartyDailyVoteCounts() =>
        await http.GetFromJsonAsync<Dictionary<Party, Dictionary<DateOnly, int>>>("api/v1/stats/daily");

    public async Task<VoterDto?> GetVoterByAddress(string address) =>
        await http.GetFromJsonAsync<VoterDto>($"api/v1/voters/{address}", Json.SerializerOptions);

    public async Task<VoteDto?> GetVoteByHash(string hash) =>
        await http.GetFromJsonAsync<VoteDto>($"api/v1/votes/{hash}", Json.SerializerOptions);

    public async Task<BlockDto?> GetBlockByIndex(long index) =>
        await http.GetFromJsonAsync<BlockDto>($"api/v1/blocks/{index}", Json.SerializerOptions);

    public async Task<BlockDto?> GetBlockByHash(string hash) =>
        await http.GetFromJsonAsync<BlockDto>($"api/v1/blocks/{hash}", Json.SerializerOptions);
}