using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Application.Dtos;

[Index(nameof(Hash), IsUnique = true)]
public record BlockDto
{
    [Key]
    [Column("idx")]
    public long Index { get; init; }

    public long Nonce { get; init; }

    public string Hash { get; init; } = default!;
    
    [Column("mrkl_root")]
    public string MerkleRoot { get; init; } = default!;
    
    public string PreviousHash { get; init; } = default!;
    
    public ICollection<VoteDto> Votes { get; init; } = [];
}