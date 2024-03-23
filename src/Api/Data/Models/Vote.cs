using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using Api.Common;
using Api.Data.Dto;
using Microsoft.EntityFrameworkCore;

namespace Api.Data.Models;

[Index(nameof(PartyId))]
public record Vote
{
    public byte[] Hash => SHA256.HashData(GetHashPayload());

    public Voter Voter { get; private init; } = default!;

    public int PartyId { get; private init; }

    public long Timestamp { get; private init; }

    public long BlockIndex { get; set; }

    public byte[] Signature { get; private set; } = default!;

    public bool VerifySignature(byte[] sig) => Voter.Verify(GetSignaturePayload(), sig);

    [Pure]
    public static Vote? TryCreate(Voter voter, int partyId, long timestamp, long blockIndex)
    {
        var vote = new Vote
        {
            Voter = voter,
            PartyId = partyId,
            Timestamp = timestamp,
            BlockIndex = blockIndex,
        };

        if (!voter.TrySign(vote.GetSignaturePayload(), out var signature)) return null;
        vote.Signature = signature;
        return vote;
    }

    public static explicit operator Vote(VoteDto dto)
    {
        var voter = Voter.FromPubKey(dto.VoterPubKey.ToBytesFromHex());
        var timestamp = new DateTimeOffset(dto.Timestamp).ToUnixTimeMilliseconds();

        var vote = new Vote
        {
            Voter = voter,
            PartyId = dto.PartyId,
            Timestamp = timestamp,
            BlockIndex = dto.BlockIndex,
            Signature = dto.Signature.ToBytesFromHex(),
        };

        if (vote.Hash.ToHexString() != dto.Hash)
            throw new InvalidOperationException("Invalid hash");

        if (!vote.VerifySignature(dto.Signature.ToBytesFromHex()))
            throw new InvalidOperationException("Invalid signature");

        return vote;
    }

    public static explicit operator VoteDto(Vote vote) => new()
    {
        Hash = vote.Hash.ToHexString(),
        VoterAddress = vote.Voter.Address,
        VoterPubKey = vote.Voter.PublicKey.ToHexString(),
        PartyId = vote.PartyId,
        Signature = vote.Signature.ToHexString(),
        Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(vote.Timestamp).UtcDateTime,
        BlockIndex = vote.BlockIndex,
    };

    public byte[] GetSignaturePayload()
    {
        var buffer = new List<byte>
        {
            Capacity = sizeof(int) + sizeof(long),
        };

        buffer.AddRange(BitConverter.GetBytes(PartyId));
        buffer.AddRange(BitConverter.GetBytes(Timestamp));

        return buffer.ToArray();
    }

    private byte[] GetHashPayload()
    {
        var buffer = new List<byte>
        {
            Capacity = 21 + sizeof(long) + sizeof(long) + 32,
        };

        buffer.AddRange(Voter.Address.ToBytesFromHex());
        buffer.AddRange(BitConverter.GetBytes(PartyId));
        buffer.AddRange(BitConverter.GetBytes(Timestamp));
        buffer.AddRange(Signature);

        return buffer.ToArray();
    }
}