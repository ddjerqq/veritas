using System.Security.Cryptography;
using Domain.Common;

namespace Domain.ValueObjects;

public record Vote(Voter Voter, int PartyId, long Timestamp)
{
    public Voter Voter { get; } = Voter.HasPrivateKey
        ? Voter
        : throw new InvalidOperationException("Cannot vote without private key");

    public byte[] Hash => SHA256.HashData(HashPayload);

    public byte[] Signature => Voter.Sign(SignaturePayload);

    public bool VerifySignature(byte[] sig) => Voter.Verify(SignaturePayload, sig);

    private byte[] SignaturePayload
    {
        get
        {
            var buffer = new List<byte>
            {
                Capacity = sizeof(int) + sizeof(long),
            };

            buffer.AddRange(BitConverter.GetBytes(PartyId));
            buffer.AddRange(BitConverter.GetBytes(Timestamp));

            return buffer.ToArray();
        }
    }

    private byte[] HashPayload
    {
        get
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
}

// public static explicit operator Vote(VoteDto dto)
// {
//     var voter = Voter.FromPubKey(dto.VoterPubKey.ToBytesFromHex());
//     var timestamp = new DateTimeOffset(dto.Timestamp).ToUnixTimeMilliseconds();
//
//     var vote = new Vote
//     {
//         Voter = voter,
//         PartyId = dto.PartyId,
//         Timestamp = timestamp,
//         Signature = dto.Signature.ToBytesFromHex(),
//     };
//
//     if (vote.Hash.ToHexString() != dto.Hash)
//         throw new InvalidOperationException("Invalid hash");
//
//     if (!vote.VerifySignature(dto.Signature.ToBytesFromHex()))
//         throw new InvalidOperationException("Invalid signature");
//
//     return vote;
// }

// public static explicit operator VoteDto(Vote vote) => new()
// {
//     Hash = vote.Hash.ToHexString(),
//     VoterAddress = vote.Voter.Address,
//     VoterPubKey = vote.Voter.PublicKey.ToHexString(),
//     PartyId = vote.PartyId,
//     Signature = vote.Signature.ToHexString(),
//     Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(vote.Timestamp).UtcDateTime,
// };