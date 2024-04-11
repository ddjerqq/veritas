using System.Security.Cryptography;
using Application.ValueObjects;
using Domain.Common;

namespace Application.Dto;


public record VoteDto(string Hash, DateTime Added, Party Party, string VoterAddress)
{
    public string ShortHash => $"{Hash[..4]}-{Hash[^4..]}";

    public string ShortVoter => VoterAddress[..16];

    public static VoteDto RandomVoteDto(string? voterAddress = null)
    {
        var party = new Party(Random.Shared.GetItems([5, 9, 36, 42], 1)[0]);

        return new VoteDto(
            RandomString(),
            DateTime.Now,
            party,
            voterAddress ?? "0x" + RandomString()[22..]);

        static string RandomString()
        {
            var buffer = new byte[16];
            Random.Shared.NextBytes(buffer);
            return SHA256.HashData(buffer).ToHexString();
        }
    }
}