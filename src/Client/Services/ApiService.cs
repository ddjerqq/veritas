using System.Diagnostics;
using System.Net.Http.Json;
using Application.Dto;
using Client.Common;

namespace Client.Services;

public class ApiService(HttpClient http, VoteService voteService)
{
    public async Task<FullVoterDto?> NewIdentity() =>
        await http.GetFromJsonAsync<FullVoterDto>("api/v1/new_identity", Json.SerializerOptions);

    public async Task<Dictionary<string, string>?> GetClaims() =>
        await http.GetFromJsonAsync<Dictionary<string, string>>("api/v1/claims");

    // TODO when we do integration, pass command here which will be created separately, using InvokeAsync
    public async Task<VoteDto?> CastVote(int partyId, CancellationToken ct = default)
    {
        Console.WriteLine("start mining vote");
        var stopwatch = Stopwatch.StartNew();

        var castVoteCommand = await voteService.CreateVoteCommand(partyId);
        Console.WriteLine($"mining took: {stopwatch.Elapsed:c}");

        var resp = await http.PostAsJsonAsync("api/v1/cast_vote", castVoteCommand, Json.SerializerOptions, ct);
        return await resp.Content.ReadFromJsonAsync<VoteDto>(Json.SerializerOptions, ct);
    }

    public async Task<Dictionary<int, int>?> GetPartyVoteCounts() =>
        await http.GetFromJsonAsync<Dictionary<int, int>>("api/v1/stats/counts");

    public async Task<Dictionary<int, Dictionary<DateOnly, int>>?> GetPartyDailyVoteCounts() =>
        await http.GetFromJsonAsync<Dictionary<int, Dictionary<DateOnly, int>>>("api/v1/stats/daily");

    public async Task<VoterDto?> GetVoterByAddress(string address) =>
        await http.GetFromJsonAsync<VoterDto>($"api/v1/voters/{address}", Json.SerializerOptions);

    public async Task<VoteDto?> GetVoteByHash(string hash) =>
        await http.GetFromJsonAsync<VoteDto>($"api/v1/votes/{hash}", Json.SerializerOptions);

    public async Task<BlockDto?> GetBlockByIndex(long index) =>
        await http.GetFromJsonAsync<BlockDto>($"api/v1/blocks/{index}", Json.SerializerOptions);

    public async Task<BlockDto?> GetBlockByHash(string hash) =>
        await http.GetFromJsonAsync<BlockDto>($"api/v1/blocks/{hash}", Json.SerializerOptions);
}