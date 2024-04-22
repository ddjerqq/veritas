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
        var voter = await voterAccessor.GetVoterAsync();
        var ts = dateTimeProvider.UtcNowUnixTimeMilliseconds;
        var payload = VoteExt.GetHashPayload(voter.Address, partyId, ts, 0);
        var foundNonce = Miner.Mine(payload, Vote.Difficulty);
        var hash = SHA256.HashData(payload).ToHexString();

        var command = new CastVoteCommand(hash, voter.PublicKey, voter.PrivateKey, partyId, ts, foundNonce);
        Console.WriteLine($"created command: {command}");

        return command;
    }
}