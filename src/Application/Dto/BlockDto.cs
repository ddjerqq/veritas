namespace Application.Dto;

public record BlockDto(long Index, DateTime Mined, IEnumerable<VoteDto> Votes)
{
    public int VoteCount => Votes.Count();

    public double SizeBytes => (double)VoteCount * 208;

    public static BlockDto RandomBlockDto(long? index = null)
    {
        return new BlockDto(
            index ?? Random.Shared.Next(0, 10_000_000),
            DateTime.Now,
            Enumerable.Range(0, Random.Shared.Next(1, 100))
                .Select(_ => VoteDto.RandomVoteDto()));
    }
}