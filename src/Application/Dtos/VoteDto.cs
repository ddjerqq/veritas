using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Application.Dtos;

[Index(nameof(VoterAddress), IsUnique = true)]
public record VoteDto
{
    [Key]
    public string Hash { get; init; } = default!;

    public string VoterAddress { get; init; } = default!;

    public string VoterPubKey { get; init; } = default!;

    public string Signature { get; init; } = default!;

    public int PartyId { get; init; }

    public DateTime Timestamp { get; init; }

    public long BlockIndex { get; set; }

    public BlockDto Block { get; init; } = default!;
}