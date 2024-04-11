using Domain.Common;
using Domain.ValueObjects;

namespace Application.Dto;


public record VoteDto(string Hash, DateTime Added, Party Party, string VoterAddress)
{
    public string ShortHash => $"{Hash[..4]}-{Hash[^4..]}";

    public string ShortVoter => VoterAddress[..16];

    public static VoteDto RandomVoteDto(string? voterAddress = null)
    {
        var party = new Party(Random.Shared.GetItems([5, 9, 36, 42], 1)[0]);

        return new VoteDto(
            StringExt.RandomHexString(64),
            DateTime.Now,
            party,
            voterAddress ?? "0x" + StringExt.RandomHexString(42));
    }
}