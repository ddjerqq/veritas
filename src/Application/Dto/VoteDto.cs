using Domain.Common;
using Domain.ValueObjects;

namespace Application.Dto;

public record VoteDto(string Hash, long Nonce, DateTime Timestamp, Party Party, string VoterAddress, long? BlockIndex)
{
    public string ShortHash => $"{Hash[..4]}-{Hash[^4..]}";

    public string ShortVoterAddress => VoterAddress[..8];

    public static VoteDto RandomVoteDto()
    {
        var party = new Party(Random.Shared.GetItems([5, 9, 36, 42], 1)[0]);

        return new VoteDto(
            StringExt.RandomHexString(64),
            Random.Shared.Next(0, 10_000_000),
            DateTime.Now,
            party,
            "0x" + StringExt.RandomHexString(42),
            null);
    }
}