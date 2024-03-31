using Application.Abstractions;
using Application.Votes.Commands;
using Domain.Common;
using Domain.Entities;
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
        var vote = Vote.NewVote(voter, 5, dateTimeProvider.UtcNowUnixTimeMilliseconds);
        vote.Mine();

        return Ok(new
        {
            addr = voter.Address,

            pkey = voter.PublicKey,
            addr_signed = voter.Sign(voter.Address.ToBytesFromHex()).ToHexString(),

            vote = new
            {
                hash = vote.Hash,
                pkey = voter.PublicKey,
                sig = vote.Signature,
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
        if (processedVotesCache.Contains(command.Hash))
        {
            logger.LogWarning("Tried processing the same vote twice: {Hash}", command.Hash);
            return BadRequest($"Tried processing the same vote twice: {command.Hash}");
        }

        var vote = await mediator.Send(command, ct);
        processedVotesCache.Add(vote.Hash);

        return Created();
    }

    [HttpPost("vote_random")]
    public async Task<IActionResult> VoteRandom(CancellationToken ct = default)
    {
        if (HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>() is var env && !env.IsDevelopment())
            return NotFound();

        var voter = Voter.NewVoter();
        var vote = Vote.NewVote(voter, Random.Shared.Next(1, 100), dateTimeProvider.UtcNowUnixTimeMilliseconds);
        vote.Mine();

        // override the test voter
        HttpContext.Items[nameof(Voter)] = voter;

        var command = new CastVoteCommand(
            vote.Hash, voter.PublicKey, vote.Signature,
            vote.PartyId, vote.Timestamp, vote.Nonce);

        if (processedVotesCache.Contains(command.Hash))
        {
            logger.LogWarning("Tried processing the same vote twice: {Hash}", command.Hash);
            return BadRequest($"Tried processing the same vote twice: {command.Hash}");
        }

        vote = await mediator.Send(command, ct);
        processedVotesCache.Add(vote.Hash);

        return Created();
    }
}

