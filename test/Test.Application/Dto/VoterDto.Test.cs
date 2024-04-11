using Application.Dto;
using Domain.ValueObjects;

namespace Test.Application.Dto;

public class VoterDtoTest
{
    [Test]
    [NonParallelizable]
    public void TestFavoriteParty()
    {
        var voterDto = VoterDto.RandomVoterDto(100);
        var counts = new Dictionary<int, int>();

        foreach (var vote in voterDto.Votes)
        {
            if (!counts.TryAdd(vote.Party.Id, 1))
                counts[vote.Party.Id]++;
        }

        Party partyA = voterDto.FavoriteParty!.Value;
        Party partyB = counts.MaxBy(pair => pair.Value).Key;

        Assert.That(partyA.Id, Is.EqualTo(partyB.Id));
    }
}