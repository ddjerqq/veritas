using Application.Abstractions;
using Application.Votes.Commands;
using Domain.Common;
using Domain.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.V1;

[Authorize]
[ApiController]
[Route("/api/v1/")]
[Produces("application/json")]
public class ApiController(
    IMediator mediator,
    ILogger<ApiController> logger,
    IProcessedVotesCache processedVotesCache,
    IDateTimeProvider dateTimeProvider) : ControllerBase
{
    [HttpGet("new_identity")]
    public IActionResult NewIdentity()
    {
        var voter = Voter.NewVoter();
        var vote = new Vote(voter, 5, dateTimeProvider.UtcNowUnixTimeMilliseconds);
        vote = vote.Mine();

        return Ok(new
        {
            addr = voter.Address,

            pkey = voter.PublicKey.ToHexString(),
            addr_signed = voter.Sign(voter.Address.ToBytesFromHex()).ToHexString(),

            vote = new
            {
                hash = vote.Hash.ToHexString(),
                pkey = voter.PublicKey.ToHexString(),
                sig = vote.Signature.ToHexString(),
                party_id = vote.PartyId,
                timestamp = vote.Timestamp,
                nonce = vote.Nonce,
            },
        });
    }

    [HttpGet("user_claims")]
    public ActionResult<Dictionary<string, string>> GetUserClaims()
    {
        return Ok(User.Claims.ToDictionary(x => x.Type, x => x.Value));
    }

    [HttpPost("vote")]
    public async Task<IActionResult> CastVote(CastVoteCommand command, CancellationToken ct = default)
    {
        var processedVote = processedVotesCache.GetByHash(command.Hash);
        if (processedVote is not null)
        {
            logger.LogWarning("Tried processing the same vote twice: {Hash}", processedVote.Hash.ToHexString());
            return BadRequest($"Tried processing the same vote twice: {processedVote.Hash.ToHexString()}");
        }

        var vote = await mediator.Send(command, ct);
        processedVotesCache.Set(vote);

        return Created();
    }

    [HttpPost("vote/{partyId:int}")]
    public async Task<IActionResult> CastVote(int partyId, CancellationToken ct = default)
    {
        if (HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>() is var env && !env.IsDevelopment())
            return NotFound();

        var voter = Voter.NewVoter();
        var minedVote = new Vote(voter, partyId, dateTimeProvider.UtcNowUnixTimeMilliseconds).Mine();

        // override the test voter
        HttpContext.Items[nameof(Voter)] = voter;

        var command = new CastVoteCommand(
            minedVote.Hash.ToHexString(),
            voter.PublicKey.ToHexString(),
            minedVote.Signature.ToHexString(),
            partyId,
            minedVote.Timestamp,
            minedVote.Nonce);

        var processedVote = processedVotesCache.GetByHash(command.Hash);
        if (processedVote is not null)
        {
            logger.LogWarning("Tried processing the same vote twice: {Hash}", processedVote.Hash.ToHexString());
            return BadRequest($"Tried processing the same vote twice: {processedVote.Hash.ToHexString()}");
        }

        var vote = await mediator.Send(command, ct);
        processedVotesCache.Set(vote);

        return Created();
    }
}

