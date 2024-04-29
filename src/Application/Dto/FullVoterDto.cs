namespace Application.Dto;

public record FullVoterDto(string Address, string PublicKey, string PrivateKey, string Signature, DateTime LastVoteTime)
{
    public DateTime LastVoteTime { get; set; } = LastVoteTime;
}