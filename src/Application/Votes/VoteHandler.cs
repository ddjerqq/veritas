using Application.Votes.Commands;
using MediatR;

namespace Application.Votes;

public class VoteHandler : IRequestHandler<CastVoteCommand, bool>
{
    public Task<bool> Handle(CastVoteCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}