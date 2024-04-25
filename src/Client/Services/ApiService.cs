using System.Net.Http.Json;
using Application.Blockchain.Commands;
using Application.Dto;
using Client.Common;

namespace Client.Services;

public class ApiService(HttpClient http)
{
    public async Task<FullVoterDto?> NewIdentity(CancellationToken ct = default) =>
        await http.GetFromJsonAsync<FullVoterDto>("api/v1/new_identity", Json.SerializerOptions, ct);

    public async Task<Dictionary<string, string>?> GetClaims(CancellationToken ct = default) =>
        await http.GetFromJsonAsync<Dictionary<string, string>>("api/v1/claims", ct);

    public async Task<VoteDto?> CastVote(CastVoteCommand command, CancellationToken ct = default)
    {
        var resp = await http.PostAsJsonAsync("api/v1/cast_vote", command, Json.SerializerOptions, ct);
        return await resp.Content.ReadFromJsonAsync<VoteDto>(Json.SerializerOptions, ct);
    }

    public async Task<Dictionary<int, int>?> GetPartyVoteCounts(CancellationToken ct = default) =>
        await http.GetFromJsonAsync<Dictionary<int, int>>("api/v1/stats/counts", ct);

    public async Task<Dictionary<int, Dictionary<DateOnly, int>>?> GetPartyDailyVoteCounts(CancellationToken ct = default) =>
        await http.GetFromJsonAsync<Dictionary<int, Dictionary<DateOnly, int>>>("api/v1/stats/daily", ct);

    public async Task<VoterDto?> GetVoterByAddress(string address, CancellationToken ct = default) =>
        await http.GetFromJsonAsync<VoterDto>($"api/v1/voters/{address}", Json.SerializerOptions, ct);

    public async Task<VoteDto?> GetVoteByHash(string hash, CancellationToken ct = default) =>
        await http.GetFromJsonAsync<VoteDto>($"api/v1/votes/{hash}", Json.SerializerOptions, ct);

    public async Task<BlockDto?> GetBlockByIndex(long index, CancellationToken ct = default) =>
        await http.GetFromJsonAsync<BlockDto>($"api/v1/blocks/{index}", Json.SerializerOptions, ct);

    public async Task<IEnumerable<BlockDto>?> GetLastNBlocks(int amount, CancellationToken ct = default) =>
        await http.GetFromJsonAsync<IEnumerable<BlockDto>>($"api/v1/blocks/last?amount={amount}", Json.SerializerOptions, ct);

    public async Task<BlockDto?> GetBlockByHash(string hash, CancellationToken ct = default) =>
        await http.GetFromJsonAsync<BlockDto>($"api/v1/blocks/{hash}", Json.SerializerOptions, ct);
}