using Domain.Common;
using Domain.ValueObjects;

namespace Application.Dto;

public record VoteDto(string Hash, long Nonce, DateTime Timestamp, int PartyId, string VoterAddress, long? BlockIndex)
{
    public Party Party => PartyId;

    public string ShortHash => $"{Hash[..4]}-{Hash[^4..]}";

    public string ShortVoterAddress => VoterAddress[..8];

    public static VoteDto RandomVoteDto()
    {
        return new VoteDto(
            StringExt.RandomHexString(64),
            Random.Shared.Next(0, 10_000_000),
            DateTime.Now,
            Random.Shared.GetItems(Party.Allowed, 1)[0],
            "0x" + StringExt.RandomHexString(42),
            null);
    }
}