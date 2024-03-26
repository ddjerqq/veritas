using AutoMapper;
using Domain.Common;
using Domain.ValueObjects;

namespace Application.Dtos;

public record VoteDto
{
    public string Hash { get; init; } = default!;

    public string VoterAddress { get; init; } = default!;

    public string VoterPubKey { get; init; } = default!;

    public string Signature { get; init; } = default!;

    public int PartyId { get; init; }

    public DateTime Timestamp { get; init; }

    public long BlockIndex { get; set; }

    public BlockDto Block { get; init; } = default!;
}


internal class VoteTypeConverter : ITypeConverter<VoteDto, Vote>, ITypeConverter<Vote, VoteDto>
{
    public Vote Convert(VoteDto source, Vote destination, ResolutionContext context)
    {
        var vote = new Vote(
            Voter.FromPubKey(source.VoterPubKey.ToBytesFromHex()),
            source.PartyId,
            new DateTimeOffset(source.Timestamp).ToUnixTimeMilliseconds())
        {
            Signature = source.Signature.ToBytesFromHex(),
        };

        if (source.Hash != vote.Hash.ToHexString())
            throw new InvalidOperationException($"failed to convert Vote, expected: {source.Hash} was: {vote.Hash.ToHexString()}");

        if (!vote.VerifySignature(source.Signature.ToBytesFromHex()))
            throw new InvalidOperationException($"failed to convert Vote, was: {vote.Signature.ToHexString()}");

        return vote;
    }

    public VoteDto Convert(Vote source, VoteDto destination, ResolutionContext context)
    {
        return new VoteDto
        {
            Hash = source.Hash.ToHexString(),
            VoterAddress = source.Voter.Address,
            VoterPubKey = source.Voter.PublicKey.ToHexString(),
            Signature = source.Signature.ToHexString(),
            PartyId = source.PartyId,
            Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(source.Timestamp).UtcDateTime,
        };
    }
}


internal class VoteDtoMappingProfile : Profile
{
    public VoteDtoMappingProfile()
    {
        CreateMap<Vote, VoteDto>().ConvertUsing<VoteTypeConverter>();
        CreateMap<VoteDto, Vote>().ConvertUsing<VoteTypeConverter>();
    }
}