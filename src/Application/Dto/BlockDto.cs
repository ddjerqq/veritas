using Domain.ValueObjects;

namespace Application.Dto;

public record BlockDto(
    long Index,
    long Nonce,
    string Hash,
    string MerkleRoot,
    string PreviousHash,
    DateTime Mined,
    List<VoteDto> Votes)
{
    public string ShortHash => $"{Hash[..4]}-{Hash[^4..]}";

    public string ShortPreviousHash => $"{PreviousHash[..4]}-{PreviousHash[^4..]}";

    public string ShortMerkleRoot => $"{MerkleRoot[..4]}-{MerkleRoot[^4..]}";

    public int SizeBytes => Votes.Count * 208;

    public Party? TopParty => Votes
        .GroupBy(vote => vote.PartyId)
        .Select(group => new
        {
            Party = group.Key,
            Count = group.Count(),
        })
        .MaxBy(partyCounts => partyCounts.Count)?
        .Party;
}