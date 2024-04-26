using System.Diagnostics;
using System.Security.Cryptography;
using Application.Blockchain.Commands;
using Application.Common.Abstractions;
using Domain.Common;
using Domain.Entities;

namespace Client.Services;

public class VoteService(VoterAccessor voterAccessor, IDateTimeProvider dateTimeProvider)
{
    public async Task<CastVoteCommand> CreateVoteCommand(int partyId)
    {
        Console.WriteLine("start mining vote");
        var voter = await voterAccessor.GetVoterAsync();
        var ts = dateTimeProvider.UtcNowUnixTimeMilliseconds;
        var payload = VoteExt.GetHashPayload(voter.Address, partyId, ts, 0);

        var stopwatch = Stopwatch.StartNew();
        var foundNonce = Miner.Mine(payload, Vote.Difficulty);
        stopwatch.Stop();

        var hash = SHA256.HashData(payload).ToHexString();
        Console.WriteLine($"mining took: {stopwatch.Elapsed:c}");

        var command = new CastVoteCommand(hash, voter.PublicKey, voter.PrivateKey, partyId, ts, foundNonce);
        return command;
    }
}