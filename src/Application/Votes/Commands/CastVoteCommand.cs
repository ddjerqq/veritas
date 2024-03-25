using Domain.ValueObjects;
using MediatR;

namespace Application.Votes.Commands;

public record CastVoteCommand(Voter Voter, int PartyId) : IRequest<bool>;