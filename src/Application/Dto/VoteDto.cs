using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Dto;

[AutoMap(typeof(Vote), ReverseMap = true)]
public record VoteDto(string Hash, long Nonce, DateTime Timestamp, int PartyId, string VoterAddress, long? BlockIndex)
{
    public Party Party => PartyId;

    public string ShortHash => $"{Hash[..4]}-{Hash[^4..]}";

    public string ShortVoterAddress => VoterAddress[..8];
}